using RocketInsights.Common.Extensions;
using RocketInsights.Common.Models;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine;
using RocketInsights.DXP.Providers.Kontent.Extensions;
using RocketInsights.DXP.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent
{
    public class LayoutService : ILayoutService
    {
        private IContextService ContextService { get; }

        private IKontentApiEngine KontentApiEngine { get; set; }

        private readonly IContentService ContentService;

        public LayoutService(IContextService contextService, IKontentApiEngine kontentApiEngine, IContentService contentService)
        {
            KontentApiEngine = kontentApiEngine;
            ContextService = contextService;
            ContentService = contentService;
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
                                    fragments.Add(await ContentService.GetFragmentAsync(fragmentsValue.ToString()).ConfigureAwait(false));
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
    }
}
