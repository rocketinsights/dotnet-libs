﻿using RocketInsights.Common.Extensions;
using RocketInsights.Common.Models;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine;
using RocketInsights.DXP.Providers.Kontent.Common;
using RocketInsights.DXP.Providers.Kontent.Extensions;
using RocketInsights.DXP.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent
{
    public class KontentService : ILayoutService, IContentService
    {
        private IContextService ContextService { get; }

        private IKontentApiEngine KontentApiEngine { get; set; }

        private readonly Dictionary<string, Fragment> processedFragments = new Dictionary<string, Fragment>();

        public KontentService(IContextService contextService, IKontentApiEngine kontentApiEngine)
        {
            ContextService = contextService;
            KontentApiEngine = kontentApiEngine;
        }

        public async Task<Composition> GetCompositionAsync()
        {
            if (!ContextService.TryGetContext(out var context))
                throw new Exception("Unable to retrieve a context");

            if (!context.Content.TryParse<Uri>("uri", out var uri))
                throw new Exception("Unable to retrive the context uri");

            var response = await KontentApiEngine.GetComposition(uri).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(response))
                throw new Exception("Unable to retrieve composition");

            using var jsonDocument = JsonDocument.Parse(response);
            var compositionItems = jsonDocument.RootElement.GetSubProperty("items");
            if (compositionItems == null || ((JsonElement)compositionItems).GetArrayLength() == 0)
                throw new Exception("Unable to retrieve composition");

            var compositionItem = ((JsonElement)compositionItems)[0];

            var composition = new Composition()
            {
                Id = compositionItem.GetSubProperty("system.codename")?.ToString(),
                Name = compositionItem.GetSubProperty("system.name")?.ToString(),
                Guid = new Guid(compositionItem.GetSubProperty("system.id")?.ToString()),
                ContentType = compositionItem.GetSubProperty("system.type")?.ToString(),
                Content = new Content()
            };

            composition.Content.Add("slug", $"/{uri}");

            var elementsProperty = compositionItem.GetSubProperty("elements");
            if (elementsProperty == null)
                return composition;

            composition.Regions = await GetRegionsAsync((JsonElement)elementsProperty).ConfigureAwait(false);

            return await Task.FromResult(composition);
        }

        private async Task<List<Region>> GetRegionsAsync(JsonElement jsonElements)
        {
            var regions = new List<Region>();

            var regionsElement = jsonElements.GetSubProperty("regions");
            if (regionsElement != null)
            {
                var regionsValues = ((JsonElement)regionsElement).GetSubProperty("value");
                if (regionsValues != null)
                {
                    foreach (var regionsValue in ((JsonElement)regionsValues).EnumerateArray())
                    {
                        var regionDetails = await KontentApiEngine.GetContentItem(regionsValue.ToString()).ConfigureAwait(false);
                        if (string.IsNullOrWhiteSpace(regionDetails))
                            continue;

                        using var jsonDocument = JsonDocument.Parse(regionDetails);
                        var regionItem = jsonDocument.RootElement.GetSubProperty("item");
                        if (regionItem == null)
                            continue;

                        var regionElements = ((JsonElement)regionItem).GetSubProperty("elements");

                        var region = new Region
                        {
                            Name = ((JsonElement)regionItem).GetSubProperty("system.name")?.ToString(),
                            ContentType = ((JsonElement)regionItem).GetSubProperty("system.type")?.ToString(),
                            Id = ((JsonElement)regionItem).GetSubProperty("system.codename")?.ToString(),
                            Regions = regionElements != null ? await GetRegionsAsync((JsonElement)regionElements).ConfigureAwait(false) : new List<Region>()
                        };

                        var fragmentsElement = regionElements != null ? ((JsonElement)regionElements).GetSubProperty("fragments") : null;
                        if (fragmentsElement != null)
                        {
                            var fragmentValues = ((JsonElement)fragmentsElement).GetSubProperty("value");
                            if (fragmentValues != null)
                            {
                                var fragments = new List<Fragment>();
                                foreach (var fragmentsValue in ((JsonElement)fragmentValues).EnumerateArray())
                                {
                                    fragments.Add(await GetFragmentAsync(fragmentsValue.ToString()));
                                }

                                region.Fragments = fragments;
                            }
                        }

                        regions.Add(region);
                    }
                }
            }

            return regions;
        }

        public async Task<Fragment> GetFragmentAsync(string id)
        {
            if (!ContextService.TryGetContext(out _))
                throw new Exception("Unable to retrieve a context");

            if (processedFragments.TryGetValue(id, out var existingFragment))
                return existingFragment;
            else
                processedFragments.Add(id, new Fragment());

            var response = await KontentApiEngine.GetContentItem(id).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(response))
                throw new Exception("Unable to retrieve fragment");

            using var jsonDocument = JsonDocument.Parse(response);
            var fragment = new Fragment()
            {
                Id = jsonDocument.RootElement.GetSubProperty("item.system.codename")?.ToString(),
                Name = jsonDocument.RootElement.GetSubProperty("item.system.name")?.ToString(),
                Guid = new Guid(jsonDocument.RootElement.GetSubProperty("item.system.id")?.ToString()),
                Template = new Template()
                {
                    Name = jsonDocument.RootElement.GetSubProperty("item.system.type")?.ToString(),
                },
                Content = new Content()
            };

            var elementsProperty = jsonDocument.RootElement.GetSubProperty("item.elements");

            if (elementsProperty == null)
                return fragment;

            var elementObjects = JsonSerializer.Deserialize<Dictionary<string, object>>((JsonElement)elementsProperty);
            if (elementObjects == null)
                return fragment;

            var fragmentElements = new Dictionary<string, object>();

            foreach (var elementObject in elementObjects)
            {
                var rootElement = JsonDocument.Parse(JsonSerializer.Serialize(elementObject.Value)).RootElement;
                var value = rootElement.GetSubProperty("value");

                if (value == null)
                    return fragment;

                fragmentElements = await GetFragmentElements(fragmentElements, rootElement, (JsonElement)value, elementObject.Key).ConfigureAwait(false);
            }

            foreach (var fragmentElement in fragmentElements)
            {
                fragment.Content.Add(fragmentElement.Key, fragmentElement.Value);
            }

            processedFragments[id] = fragment;
            return fragment;
        }

        private async Task<Dictionary<string, object>> GetFragmentElements(Dictionary<string, object> fragmentElements,
            JsonElement rootElement,
            JsonElement value,
            string key)
        {
            var elementType = rootElement.GetProperty("type").GetString();

            if (value.ValueKind != JsonValueKind.Array || elementType == ElementTypeConstants.Asset)
            {
                if (elementType == ElementTypeConstants.Asset)
                    fragmentElements.Add(key, rootElement.GetSubProperty("url") ?? new object());
                else
                    fragmentElements.Add(key, JsonSerializer.Deserialize<object>(value) ?? new object());
            }
            else
                fragmentElements = await ProcessJsonElementArray(value, fragmentElements, key, elementType).ConfigureAwait(false);

            return fragmentElements;
        }

        private async Task<Dictionary<string, object>> ProcessJsonElementArray(JsonElement value,
            Dictionary<string, object> fragmentElements,
            string key,
            string? elementType)
        {
            if (value.ValueKind == JsonValueKind.Array &&
                (elementType == ElementTypeConstants.MultipleChoice || elementType == ElementTypeConstants.Taxonomy))
            {
                var values = new List<string>();
                foreach (var item in value.EnumerateArray())
                {
                    var name = item.GetSubProperty("name");

                    if (name != null)
                        values.Add(name.ToString());
                }

                fragmentElements.Add(key, values);
            }
            else
            {
                foreach (var item in value.EnumerateArray())
                {
                    var fragmentDetails = await GetFragmentAsync(item.ToString()).ConfigureAwait(false);

                    if (fragmentElements.TryGetValue(key, out var keyElement))
                    {
                        var type = keyElement.GetType();
                        if (type.IsArray)
                        {
                            var keyElements = ((IEnumerable)keyElement).Cast<Fragment>().ToList();
                            keyElements.Add(fragmentDetails);
                            fragmentElements[key] = keyElement;
                        }
                        else
                            fragmentElements[key] = new object[] { keyElement, fragmentDetails };
                    }
                    else
                        fragmentElements.Add(key, fragmentDetails);
                }
            }

            return fragmentElements;
        }
    }
}
