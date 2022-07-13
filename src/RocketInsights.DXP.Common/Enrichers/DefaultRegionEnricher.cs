using Microsoft.Extensions.DependencyInjection;
using RocketInsights.Common.Patterns.Pipelines;
using RocketInsights.DXP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Enrichers
{
    public class DefaultRegionEnricher : IChainableOperation<Region>
    {
        private IServiceProvider ServiceProvider { get; }

        public DefaultRegionEnricher(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task<Region> InvokeAsync(Region input)
        {
            input.Regions = await EnrichRegions(input.Regions);
            input.Fragments = await EnrichFragments(input.Fragments);

            return input;
        }

        private async Task<IEnumerable<Region>> EnrichRegions(IEnumerable<Region> input)
        {
            var regions = new List<Region>();

            foreach (var r in input)
            {
                var region = r;

                foreach (var enricher in ServiceProvider.GetServices<IChainableOperation<Region>>())
                {
                    try
                    {
                        region = await enricher.InvokeAsync(region);
                    }
                    catch (Exception)
                    {

                    }
                }

                regions.Add(region);
            }

            return regions;
        }

        private async Task<IEnumerable<Fragment>> EnrichFragments(IEnumerable<Fragment> input)
        {
            var fragments = new List<Fragment>();

            foreach (var f in input)
            {
                var fragment = f;

                foreach (var enricher in ServiceProvider.GetServices<IChainableOperation<Fragment>>())
                {
                    try
                    {
                        fragment = await enricher.InvokeAsync(fragment);
                    }
                    catch (Exception)
                    {

                    }
                }

                fragments.Add(fragment);
            }

            return fragments;
        }
    }
}
