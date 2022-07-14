using RocketInsights.DXP.Providers.Kontent;
using RocketInsights.DXP.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddKontent(this IServiceCollection services)
        {
            services.AddSingleton<ILayoutService, KontentService>();
            services.AddSingleton<IContentService, KontentService>();

            return services;
        }
    }
}
