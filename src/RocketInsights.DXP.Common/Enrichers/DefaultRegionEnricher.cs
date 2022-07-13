using RocketInsights.Common.Patterns.Pipelines;
using RocketInsights.DXP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Enrichers
{
    public class DefaultRegionEnricher : IChainableOperation<Region>
    {
        protected IEnumerable<IChainableOperation<Region>> RegionEnrichers { get; }
        protected IEnumerable<IChainableOperation<Fragment>> FragmentEnrichers { get; }

        public DefaultRegionEnricher(IEnumerable<IChainableOperation<Region>> regionEnrichers, IEnumerable<IChainableOperation<Fragment>> fragmentEnrichers)
        {
            RegionEnrichers = regionEnrichers;
            FragmentEnrichers = fragmentEnrichers;
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

                foreach (var enricher in RegionEnrichers)
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

                foreach (var enricher in FragmentEnrichers)
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
