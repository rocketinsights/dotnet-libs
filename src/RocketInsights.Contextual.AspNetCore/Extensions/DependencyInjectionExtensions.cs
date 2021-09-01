using Microsoft.AspNetCore.Builder;
using RocketInsights.Common.Patterns;
using RocketInsights.Contextual;
using RocketInsights.Contextual.AspNetCore;
using RocketInsights.Contextual.AspNetCore.Factories;
using RocketInsights.Contextual.AspNetCore.Middleware;
using RocketInsights.Contextual.Factories;
using RocketInsights.Contextual.Models;
using RocketInsights.Contextual.Services;
using System.Globalization;
using System.Security.Claims;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddContextual(this IServiceCollection services)
        {
            //services.AddHttpContextAccessor();
            services.AddSingleton<IContextStore, HttpContextStore>();

            services.AddSingleton<IFactory<CultureInfo>, ThreadCultureFactory>();
            services.AddSingleton<IFactory<ClaimsIdentity>, HttpHeadersIdentityFactory>();
            services.AddSingleton<IFactory<Context>, ContextFactory>(); // The factories above should come from options instead of being fixed
            
            services.AddSingleton<IContextService, ContextService>();

            return services;
        }

        public static void UseContextual(this IApplicationBuilder app)
        {
            app.UseMiddleware<ContextualMiddleware>();
        }
    }
}