using RocketInsights.Common.Patterns.Pipelines;
using RocketInsights.DXP.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Enrichers
{
    public class DefaultCompositionEnricher : IChainableOperation<Composition>
    {
        protected IEnumerable<IChainableOperation<Region>> RegionEnrichers { get; }

        public DefaultCompositionEnricher(IEnumerable<IChainableOperation<Region>> regionEnrichers)
        {
            RegionEnrichers = regionEnrichers;
        }

        public async Task<Composition> InvokeAsync(Composition input)
        {
            input.Regions = await EnrichRegions(input.Regions);

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
    }
}
