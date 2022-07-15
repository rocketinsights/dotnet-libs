using RestSharp;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine;
using RocketInsights.DXP.Services;
using System;
using System.Collections.Generic;
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

        public Task<Composition> GetCompositionAsync()
        {
            if(!ContextService.TryGetContext(out var context))
            {
                throw new Exception("Unable to retrieve a context");
            }

            var composition = new Composition()
            {
                Name = $"This came from a Kontent provider ({context.Culture.DisplayName})."
            };

            return Task.FromResult(composition);
        }

        public async Task<Fragment> GetFragmentAsync(string id)
        {
            if (!ContextService.TryGetContext(out var context))
            { 
                throw new Exception("Unable to retrieve a context");
            }

            var response = await RestRunner.Execute(
                new RestRequestProperties
                {
                    BaseUrl = new Uri("https://deliver.kontent.ai/a67bb8d5-9520-00f7-8e76-952d8123356e"),
                    Resource = "items/{codename}",
                    UrlSegments = new List<UrlSegment> { new UrlSegment("codename", "title_test") },
                    Method = Method.Get
                }).ConfigureAwait(false);

            var fragment = new Fragment()
            {
                Id = "item.system.codename",
                Name = $"This came from a Kontent provider ({context.Culture.DisplayName}).",
                Template = new Template()
                {
                    Name = "item.system.type"
                }
            };

            return await Task.FromResult(fragment);
        }
    }
}
