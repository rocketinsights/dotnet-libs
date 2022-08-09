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

            var content = response.Deserialize<Content>();

            var compositionItems = content.GetValueAsObject("items");
            if (compositionItems == null)
                throw new Exception("Unable to retrieve composition");

            var compositionDictionary = ((IList<object>)compositionItems)[0] as Dictionary<string, object>;
            var systemDictionary = compositionDictionary?.GetValueAsDictionary("system");

            var composition = new Composition()
            {
                Id = systemDictionary?.GetValueAsObject("codename")?.ToString(),
                Name = systemDictionary?.GetValueAsObject("name")?.ToString(),
                Guid = new Guid(systemDictionary?.GetValueAsObject("id")?.ToString()),
                ContentType = systemDictionary?.GetValueAsObject("type")?.ToString(),
                Content = new Content()
            };

            composition.Content.Add("slug", $"/{uri}");

            if (compositionDictionary == null)
                return composition;

            if (!(compositionDictionary.GetValueAsObject("elements") is Dictionary<string, object> elementsDictionary))
                return composition;

            composition.Regions = await GetRegionsAsync(elementsDictionary).ConfigureAwait(false);

            return await Task.FromResult(composition);
        }

        private async Task<List<Region>> GetRegionsAsync(Dictionary<string, object>? elementsDictionary)
        {
            var regions = new List<Region>();
            if (elementsDictionary == null)
                return regions;

            if (!(elementsDictionary.GetValueAsObject("regions") is Dictionary<string, object> regionsDictionary))
                return regions;

            if (!( regionsDictionary.GetValueAsObject("value") is List<object> regionsValues))
                return regions;

            foreach (var regionsValue in regionsValues)
            {
                var regionDetails = await KontentApiEngine.GetContentItem(regionsValue.ToString()).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(regionDetails))
                    continue;

                var content = regionDetails.Deserialize<Content>();

                if (!(content.GetValueAsObject("item") is Dictionary<string, object> regionItem))
                    continue;

                var systemDictionary = regionItem.GetValueAsDictionary("system");
                var regionElements = regionItem.GetValueAsDictionary("elements");

                var region = new Region
                {
                    Name = systemDictionary?.GetValueAsObject("name")?.ToString(),
                    ContentType = systemDictionary?.GetValueAsObject("type")?.ToString(),
                    Id = systemDictionary?.GetValueAsObject("codename")?.ToString(),
                    Regions = regionElements != null 
                        ? await GetRegionsAsync(regionElements).ConfigureAwait(false) 
                        : new List<Region>()
                };

                var fragmentsElement = regionElements != null 
                    ? regionElements.GetValueAsDictionary("fragments") 
                    : null;

                if (fragmentsElement != null &&  fragmentsElement.GetValueAsObject("value") is List<object> fragmentValues)
                {
                    var fragments = new List<Fragment>();
                    foreach (var fragmentsValue in fragmentValues)
                    {
                        fragments.Add(await ContentService.GetFragmentAsync(fragmentsValue.ToString()).ConfigureAwait(false));
                    }

                    region.Fragments = fragments;
                }

                regions.Add(region);
            }

            return regions;
        }
    }
}
