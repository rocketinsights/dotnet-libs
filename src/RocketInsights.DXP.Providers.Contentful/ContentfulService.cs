using Contentful.Core.Models;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Contentful.Proxy;
using RocketInsights.DXP.Providers.Contentful.Types;
using RocketInsights.DXP.Services;
using System;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Contentful
{
    public class ContentfulService : IExperienceService
    {
        private IContextService ContextService { get; }
        private IContentfulProxy ContentfulProxy { get; }

        public ContentfulService(IContextService contextService, IContentfulProxy contentfulProxy)
        {
            ContextService = contextService;
            ContentfulProxy = contentfulProxy;
        }

        public Task<Composition> GetCompositionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Fragment> GetFragmentAsync(string id)
        {
            Entry<object> fragmentEntry = ContentfulProxy.GetEntryAsync<object>(id).Result;
            throw new NotImplementedException();
        }
    }
}
