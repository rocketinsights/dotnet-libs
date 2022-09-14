using Microsoft.Extensions.Caching.Distributed;
using RocketInsights.Common.Extensions;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.Common
{
    public class CacheHelpers : ICacheHelpers
    {
        private readonly IDistributedCache Cache;

        private const string UriCodenameMappingsCacheName = "uri_codename_mappings";

        public CacheHelpers(IDistributedCache cache)
        {
            Cache = cache;
        }

        public async Task<CodenameDetails> GetCodename(string url)
        {
            var urlSegments = url.Replace("//", "/").Split('/');
            var checkedUrlSegments = "";

            var mappings = await GetCodenameMappings().ConfigureAwait(false);
            if (mappings == null)
                return new CodenameDetails { SegmentsToCheck = url };

            for (var i = urlSegments.Length; i > 0; i--)
            {
                var urlToCheck = string.Join('/', urlSegments.Take(i));

                if (i > 0 && mappings.TryGetValue(urlToCheck, out var codename))
                {
                    return new CodenameDetails
                    {
                        Codename = codename,
                        CodenameUrl = urlToCheck,
                        SegmentsToCheck = checkedUrlSegments
                    };
                }
                else if (i == 0 && urlSegments.Length > 0 && urlSegments[0] != "index" 
                    && mappings.TryGetValue("/index", out var rootCodename))
                {
                    return new CodenameDetails
                    {
                        Codename = rootCodename,
                        CodenameUrl = "/",
                        SegmentsToCheck = url
                    };
                }

                checkedUrlSegments = $"/{string.Join('/', urlSegments.Skip(i).Take(urlSegments.Length - i))}";
            }

            return new CodenameDetails
            {
                SegmentsToCheck = checkedUrlSegments,
                CodenameUrl = "/",
                Codename = "index"
            };
        }

        public async Task SaveCodename(string url, string? codename)
        {
            if (string.IsNullOrWhiteSpace(codename))
                return;

            var mappings = await GetCodenameMappings().ConfigureAwait(false);
            if (mappings.ContainsKey(url))
                mappings.Remove(url);

            mappings.Add(url, codename);

            await Cache.RemoveAsync(UriCodenameMappingsCacheName);
            await Cache.SetAsync(UriCodenameMappingsCacheName, mappings.ToByteAray()).ConfigureAwait(false);
        }

        public async Task<Composition?> GetComposition(string? codename)
        {
            if (string.IsNullOrWhiteSpace(codename))
                return null;

            var composition = await Cache.GetAsync($"{codename}_composition").ConfigureAwait(false);
            return composition?.Deserialize<Composition>();
        }

        public async Task SaveComposition(string? codename, Composition? composition)
        {
            if (string.IsNullOrWhiteSpace(codename))
                return;

            await Cache.SetAsync($"{codename}_composition", composition.ToByteAray()).ConfigureAwait(false);
        }

        private async Task<Dictionary<string, string>> GetCodenameMappings()
        {
            var mappings = await Cache.GetAsync(UriCodenameMappingsCacheName).ConfigureAwait(false);
            if (mappings == null || mappings.Length == 0)
            {
                await Cache.SetAsync(UriCodenameMappingsCacheName, new Dictionary<string, string>().ToByteAray()).ConfigureAwait(false);
                return new Dictionary<string, string>();
            }

            return mappings.Deserialize<Dictionary<string, string>>();
        }
    }
}
