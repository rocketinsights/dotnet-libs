using RocketInsights.Common.Extensions;
using RocketInsights.Common.Models;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine;
using RocketInsights.DXP.Providers.Kontent.Common;
using RocketInsights.DXP.Providers.Kontent.Extensions;
using RocketInsights.DXP.Providers.Kontent.Models;
using RocketInsights.DXP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent
{
    public class LayoutService : ILayoutService
    {
        private IContextService ContextService { get; }

        private IKontentApiEngine KontentApiEngine { get; set; }

        private readonly IContentService ContentService;
        private readonly ICacheHelpers CacheHelpers;

        public LayoutService(IContextService contextService, IKontentApiEngine kontentApiEngine, IContentService contentService,
            ICacheHelpers cacheHelpers)
        {
            KontentApiEngine = kontentApiEngine;
            ContextService = contextService;
            ContentService = contentService;
            CacheHelpers = cacheHelpers;
        }

        public async Task<Composition> GetCompositionAsync()
        {
            if (!ContextService.TryGetContext(out var context))
                throw new Exception("Unable to retrieve a context");

            if (!context.Content.TryParse<Uri>("uri", out var uri))
                throw new Exception("Unable to retrive the context uri");

            if (uri.ToString() == "/")
                uri = new Uri("/index");

            if (uri.ToString().Substring(0, 1) != "/")
                uri = new Uri($"/{uri}");

            var codenameDetails = await CacheHelpers.GetCodename(uri.ToString()).ConfigureAwait(false);
            var composition = codenameDetails.CodenameUrl == uri.ToString() 
                ? await CacheHelpers.GetComposition(codenameDetails.Codename).ConfigureAwait(false) 
                : null;

            if (composition != null)
                return composition;

            if (codenameDetails.CodenameUrl != uri.ToString())
                codenameDetails = await FindUrlCodename(codenameDetails).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(codenameDetails.Codename))
                throw new Exception("Page not found (404)");

            var response = await KontentApiEngine.GetComposition(codenameDetails.Codename).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(response))
                throw new Exception("Unable to retrieve composition");

            var content = response.Deserialize<Content>();

            if (!(content.GetValueAsObject("item") is Dictionary<string, object> item))
                throw new Exception("Unable to retrieve composition");

            var systemDictionary = item?.GetValueAsDictionary("system");

            composition = new Composition()
            {
                Id = codenameDetails.Codename,
                Name = systemDictionary?.GetValueAsObject("name")?.ToString(),
                Guid = new Guid(systemDictionary?.GetValueAsObject("id")?.ToString()),
                ContentType = systemDictionary?.GetValueAsObject("type")?.ToString(),
                Content = new Content()
            };

            composition.Content.Add("slug", uri.ToString());

            if (!(item.GetValueAsObject("elements") is Dictionary<string, object> elementsDictionary))
            {
                await CacheHelpers.SaveComposition(codenameDetails.Codename, composition).ConfigureAwait(false);
                return composition;
            }

            composition.Regions = await GetRegionsAsync(elementsDictionary).ConfigureAwait(false);
            await CacheHelpers.SaveComposition(codenameDetails.Codename, composition).ConfigureAwait(false);

            return await Task.FromResult(composition);
        }

        private async Task<List<Region>> GetRegionsAsync(Dictionary<string, object>? elementsDictionary)
        {
            var regions = new List<Region>();
            if (elementsDictionary == null)
                return regions;

            if (!(elementsDictionary.GetValueAsObject("regions") is Dictionary<string, object> regionsDictionary) ||
                !(regionsDictionary.GetValueAsObject("value") is List<object> regionsValues))
            {
                return regions;
            }

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

                var fragmentsElement = regionElements?.GetValueAsDictionary("fragments");
                if (fragmentsElement != null && fragmentsElement.GetValueAsObject("value") is List<object> fragmentValues)
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

        private async Task<CodenameDetails> FindUrlCodename(CodenameDetails codenameDetails)
        {
            if (string.IsNullOrWhiteSpace(codenameDetails.SegmentsToCheck))
                return new CodenameDetails();

            var currentUrlSlug = GetInitialUrlSlug(codenameDetails.CodenameUrl);
            var urlSegmentsToCheck = GetUrlSegmentsToCheck(codenameDetails.CodenameUrl, codenameDetails.SegmentsToCheck)?.Split('/');
            if (urlSegmentsToCheck == null || urlSegmentsToCheck.Length == 0)
                return new CodenameDetails();        

            for (var i = 0; i < urlSegmentsToCheck.Length - 1; i++)
            {
                var currentSegment = string.IsNullOrWhiteSpace(urlSegmentsToCheck[i]) || urlSegmentsToCheck[i] == "" || urlSegmentsToCheck[i] == "/"
                    ? "index"
                    : urlSegmentsToCheck[i];

                var response = await KontentApiEngine.GetSubpages(currentSegment).ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(response))
                    throw new Exception("Page not found (404)");

                var content = response.Deserialize<Content>();

                var pageItems = content.GetValueAsObject("items");
                if (pageItems == null)
                    throw new Exception("Page not found (404)");

                var pageDictionary = ((IList<object>)pageItems)[0] as Dictionary<string, object>;
                var elementsDictionary = pageDictionary?.GetValueAsDictionary("elements");

                var subpagesElement = elementsDictionary?.GetValueAsDictionary("subpages");
                if (subpagesElement == null)
                    throw new Exception("Page not found (404)");

                var subpagesValue = subpagesElement.GetValueAsObject("value");
                if (subpagesValue == null || !(subpagesValue is List<object> subpages))
                    throw new Exception("Page not found (404)");

                foreach (var subpage in subpages)
                {
                    var subpageResponse = await KontentApiEngine.GetContentItem(subpage.ToString(), "url_slug").ConfigureAwait(false);
                    if (string.IsNullOrWhiteSpace(subpageResponse))
                        continue;

                    var subpageContent = subpageResponse.Deserialize<Content>();

                    if (!(subpageContent.GetValueAsObject("item") is Dictionary<string, object> subpageItem))
                        continue;

                    var subpageElements = subpageItem.GetValueAsDictionary("elements");
                    var urlSlugElement = subpageElements?.GetValueAsDictionary("url_slug");
                    var urlSlugValue = urlSlugElement?.GetValueAsObject("value");
                    if (urlSlugValue == null)
                        continue;

                    await CacheHelpers.SaveCodename(CombineSlugs(currentUrlSlug, urlSlugValue.ToString()), subpage.ToString()).ConfigureAwait(false);
                }

                currentUrlSlug = CombineSlugs(currentUrlSlug, urlSegmentsToCheck[i + 1]);
                var currentUrlSlugCodenameDetails = await CacheHelpers.GetCodename(currentUrlSlug).ConfigureAwait(false);

                if (i == urlSegmentsToCheck.Length - 2 && !string.IsNullOrWhiteSpace(currentUrlSlugCodenameDetails?.Codename))
                    return currentUrlSlugCodenameDetails;

                if (string.IsNullOrWhiteSpace(currentUrlSlugCodenameDetails?.Codename))
                    throw new Exception("Page not found (404)");
            }

            return new CodenameDetails();
        }

        private string CombineSlugs(string? currentSlug, string slugToAdd)
        {
            return $"{currentSlug}{(!string.IsNullOrWhiteSpace(currentSlug) && currentSlug.EndsWith('/') ? "" : "/")}{slugToAdd}";
        }

        private string? GetInitialUrlSlug(string? url)
        {
            if (url == null || url.Length == 1)
                return url;

            if (url.LastIndexOf('/') == url.Length - 1)
                url = url[0..^1];

            return url.Substring(0, url.LastIndexOf('/'));
        }

        private string? GetUrlSegmentsToCheck(string? codenameUrl, string? segmentsToCheck)
        {
            if (string.IsNullOrWhiteSpace(codenameUrl) || codenameUrl == "/" || codenameUrl == "/index")
                return !string.IsNullOrWhiteSpace(segmentsToCheck) && segmentsToCheck.Substring(0, 1) != "/" 
                    ? $"/{segmentsToCheck}" 
                    : segmentsToCheck;

            return $"/{codenameUrl.Split('/').TakeLast(1)}/{string.Join('/', segmentsToCheck?.Split('/'))}";
        }
    }
}
