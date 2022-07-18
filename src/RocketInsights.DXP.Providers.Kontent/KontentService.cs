using RestSharp;
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
            var elementsProperty = jsonDocument.RootElement.GetSubProperty("item.elements");
            var contentElements = elementsProperty.DeserializeElements();

            var content = new Content();

            foreach (var contentElement in contentElements)
            {
                content.Add(contentElement.Key, contentElement.Value);
            }

            return new Fragment()
            {
                Id = jsonDocument.RootElement.GetSubProperty("item.system.codename")?.ToString(),
                Name = jsonDocument.RootElement.GetSubProperty("item.system.name")?.ToString(),
                Guid = new Guid(jsonDocument.RootElement.GetSubProperty("item.system.id")?.ToString()),
                Template = new Template()
                {
                    Name = jsonDocument.RootElement.GetSubProperty("item.system.type")?.ToString(),
                },
                Content = content
            };
        }
    }
}
