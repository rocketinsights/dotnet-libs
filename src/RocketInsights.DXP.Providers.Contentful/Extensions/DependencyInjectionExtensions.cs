using Contentful.Core;
using Contentful.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RocketInsights.DXP.Providers.Contentful.Proxy;
using RocketInsights.DXP.Services;
using System.Net.Http;

namespace RocketInsights.DXP.Providers.Contentful.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddContentful (this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IContentfulClient>(s => InitContentfulClient(configuration));

            services.AddSingleton<IContentService, ContentService>();
            services.AddSingleton<IContentfulProxy, ContentfulProxy>();

            return services;
        }

        private static ContentfulClient InitContentfulClient (IConfiguration configuration)
        {
            var httpClient = new HttpClient();
            ContentfulOptions contentfulOptions = configuration.GetSection(nameof(ContentfulOptions)).Get<ContentfulOptions>();

            ContentfulClient contentfulClient = new ContentfulClient(httpClient, contentfulOptions);
            return contentfulClient;
        }
    }
}
