using RocketInsights.DXP.Providers.Kontent;
using RocketInsights.DXP.Providers.Kontent.ApiRunnerEngine;
using RocketInsights.DXP.Services;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddKontent(this IServiceCollection services)
        {
            services.AddSingleton<ILayoutService, LayoutService>();
            services.AddSingleton<IContentService, ContentService>();
            services.AddSingleton<IRestRunner, RestRunner>();
            services.AddSingleton<IKontentApiEngine, KontentApiEngine>();

            return services;
        }
    }
}
