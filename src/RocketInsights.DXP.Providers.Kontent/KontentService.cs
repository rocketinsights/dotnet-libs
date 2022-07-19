using RestSharp;
using RocketInsights.Common.Models;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine;
using RocketInsights.DXP.Providers.Kontent.Common;
using RocketInsights.DXP.Providers.Kontent.Extensions;
using RocketInsights.DXP.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent
{
    public class KontentService : ILayoutService, IContentService
    {
        private IContextService ContextService { get; }

        private IRestRunner RestRunner { get; set; }

        public KontentService(IContextService contextService, IRestRunner restRunner)
        {
            ContextService = contextService;
            RestRunner = restRunner;
        }

        public async Task<Composition> GetCompositionAsync()
        {
            if (!ContextService.TryGetContext(out var context))
                throw new Exception("Unable to retrieve a context");

            var composition = new Composition()
            {
                Name = $"This came from a Kontent provider ({context.Culture.DisplayName})."
            };

            return await Task.FromResult(composition);
        }

        public async Task<Fragment> GetFragmentAsync(string id)
        {
            if (!ContextService.TryGetContext(out _))
                throw new Exception("Unable to retrieve a context");

            var response = await RestRunner.Execute(
                new RestRequestProperties
                {
                    BaseUrl = new Uri(UrlHelpers.GetKontentBaseUrl("a67bb8d5-9520-00f7-8e76-952d8123356e")),
                    Resource = "items/{codename}",
                    UrlSegments = new List<UrlSegment> { new UrlSegment("codename", id) },
                    Method = Method.Get
                }).ConfigureAwait(false);

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
            else if (value.ValueKind == JsonValueKind.Array &&
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
                    fragmentElements.Add(key, fragmentDetails);
                }
            }

            return fragmentElements;
        }
    }
}
