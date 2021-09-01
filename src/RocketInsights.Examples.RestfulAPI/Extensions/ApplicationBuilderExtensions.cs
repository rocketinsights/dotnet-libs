using Microsoft.AspNetCore.Builder;
using System.Linq;

namespace RocketInsights.Examples.RestfulAPI.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureLocalization(this IApplicationBuilder app)
        {
            var supportedCultures = new[] { "en-US", "fr-CA" };

            var localizationOptions = new RequestLocalizationOptions
            {
                ApplyCurrentCultureToResponseHeaders = true
            };

            localizationOptions
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures)
                .SetDefaultCulture(supportedCultures.First());

            app.UseRequestLocalization(localizationOptions);
        }
    }
}
