using RocketInsights.Common.Models;
using RocketInsights.Contextual.Services;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Providers.Contentful.Proxy;
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
            if (ContextService.TryGetContext(out var context))
            {
                var fragmentEntry = ContentfulProxy.GetEntryByIdAsync<Content>(id, context.Culture?.Name ?? "en-US").Result;
                if (fragmentEntry != null)
                {
                    var fragment = new Fragment()
                    {
                        Id = fragmentEntry.SystemProperties.Id,
                        ContentType = fragmentEntry.SystemProperties.ContentType.Name,
                        Content = fragmentEntry.Fields
                    };
                    return Task.FromResult(fragment);
                }
                else
                {
                    throw new Exception($"Unable to fetch Fragment: {id}");
                }
            }
            else
            {
                throw new Exception("Unable to fetch context");
            }
        }
    }
}
