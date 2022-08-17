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

        public async Task<string?> GetContentItem(string codename)
        {
            return await RestRunner.Execute(
                new RestRequestProperties
                {
                    BaseUrl = new Uri(UrlHelpers.GetKontentBaseUrl(ProjectId)),
                    Resource = "items/{codename}",
                    UrlSegments = new List<UrlSegment> { new UrlSegment("codename", codename) },
                    Method = Method.Get
                }).ConfigureAwait(false);
        }

        public async Task<string?> GetComposition(Uri uri)
        {
            return await RestRunner.Execute(
                new RestRequestProperties
                {
                    BaseUrl = new Uri(UrlHelpers.GetKontentBaseUrl(ProjectId)),
                    Resource = "items",
                    QueryParameters = new Dictionary<string, string>
                    {
                        { "elements", "url_slug,name,regions" },
                        { "system.type[eq]", "composition" },
                        { "elements.url_slug", uri.ToString() }
                    },
                    Method = Method.Get
                }).ConfigureAwait(false);
        }
    }
}
