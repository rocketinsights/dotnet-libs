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
            services.AddSingleton<ILayoutService, KontentService>();
            services.AddSingleton<IContentService, KontentService>();
            services.AddSingleton<IRestRunner, RestRunner>();

            return services;
        }
    }
}
