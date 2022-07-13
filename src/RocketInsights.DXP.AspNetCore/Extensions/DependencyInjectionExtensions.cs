using RocketInsights.Common.Patterns;
using RocketInsights.Common.Patterns.Pipelines;
using RocketInsights.DXP.Enrichers;
using RocketInsights.DXP.Factories;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDXP(this IServiceCollection services)
        {
            services.AddSingleton<IFactory<Composition>, DefaultCompositionFactory>();

            services.AddSingleton<IChainableOperation<Composition>, DefaultCompositionEnricher>();
            services.AddSingleton<IChainableOperation<Region>, DefaultRegionEnricher>();

            services.AddSingleton<IFactory<ILayoutService>, SingleDependencyFactory<ILayoutService>>();
            services.AddSingleton<IFactory<IContentService>, SingleDependencyFactory<IContentService>>();
            
            services.AddSingleton<IExperienceService, DefaultExperienceService>();

            return services;
        }
    }
}
