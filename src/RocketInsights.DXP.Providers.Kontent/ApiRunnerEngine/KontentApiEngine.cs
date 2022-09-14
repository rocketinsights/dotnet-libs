using RestSharp;
using RocketInsights.DXP.Providers.Kontent.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine
{
    public class KontentApiEngine : IKontentApiEngine
    {
        private const string ProjectId = "a67bb8d5-9520-00f7-8e76-952d8123356e";

        private readonly IRestRunner RestRunner;

        public KontentApiEngine(IRestRunner restRunner)
        {
            RestRunner = restRunner;
        }

        public async Task<string?> GetContentItem(string codename, 
            string? elements = null, 
            string? systemType = null)
        {
            var request = new RestRequestProperties
            {
                BaseUrl = new Uri(UrlHelpers.GetKontentBaseUrl(ProjectId)),
                Resource = "items/{codename}",
                UrlSegments = new List<UrlSegment> { new UrlSegment("codename", codename) },
                QueryParameters = new Dictionary<string, string>(),
                Method = Method.Get                
            };

            if (!string.IsNullOrWhiteSpace(elements))
                request.QueryParameters.Add("elements", elements);

            if (!string.IsNullOrWhiteSpace(systemType))
                request.QueryParameters.Add("system.type[eq]", systemType);

            return await RestRunner.Execute(request).ConfigureAwait(false);
        }

        public async Task<string?> GetComposition(string codename)
        {
            return await GetContentItem(codename, "url_slug,name,regions", "composition").ConfigureAwait(false);
        }

        public async Task<string?> GetSubpages(string url)
        {
            return await RestRunner.Execute(
                new RestRequestProperties
                {
                    BaseUrl = new Uri(UrlHelpers.GetKontentBaseUrl(ProjectId)),
                    Resource = "items",
                    QueryParameters = new Dictionary<string, string>
                    {
                        { "elements", "subpages" },
                        { "elements.url_slug", url }
                    },
                    Method = Method.Get
                }).ConfigureAwait(false);
        }
    }
}
