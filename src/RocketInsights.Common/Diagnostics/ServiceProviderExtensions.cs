using RocketInsights.Common.Diagnostics;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection AddProfiler(this IServiceCollection services, Action<ProfilerManagerOptions> configureOptions = default)
        {
            if (configureOptions != default)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<ProfilerManagerOptions>(options =>
                {
                    // Use defaults
                });
            }

            services.AddSingleton<ProfilerManager>();

            return services;
        }
    }
}
