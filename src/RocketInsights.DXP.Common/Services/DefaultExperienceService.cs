using RocketInsights.Common.Patterns;
using RocketInsights.Common.Patterns.Pipelines;
using RocketInsights.DXP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Services
{
    public class DefaultExperienceService : IExperienceService
    {
        protected IFactory<Composition> CompositionFactory { get; }
        protected IFactory<IContentService> ContentServiceFactory { get; }

        protected IEnumerable<IChainableOperation<Composition>> CompositionEnrichers { get; }
        protected IEnumerable<IChainableOperation<Fragment>> FragmentEnrichers { get; }

        public DefaultExperienceService(IFactory<Composition> compositionFactory, IFactory<IContentService> contentServiceFactory, IEnumerable<IChainableOperation<Composition>> compositionEnrichers, IEnumerable<IChainableOperation<Fragment>> fragmentEnrichers)
        {
            CompositionFactory = compositionFactory;
            ContentServiceFactory = contentServiceFactory;
            CompositionEnrichers = compositionEnrichers;
            FragmentEnrichers = fragmentEnrichers;
        }

        public async Task<Composition> GetCompositionAsync()
        {
            var composition = await CompositionFactory.Create();

            foreach(var enricher in CompositionEnrichers)
            {
                try
                {
                    composition = await enricher.InvokeAsync(composition);
                }
                catch(Exception)
                {

                }
            }

            return composition;
        }

        public async Task<Fragment> GetFragmentAsync(string id)
        {
            var contentService = await ContentServiceFactory.Create();

            var fragment = await contentService.GetFragmentAsync(id);

            foreach(var enricher in FragmentEnrichers)
            {
                try
                {
                    fragment = await enricher.InvokeAsync(fragment);
                }
                catch(Exception)
                {

                }
            }

            return fragment;
        }
    }
}
