using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RocketInsights.Common.Models;
using RocketInsights.Contextual;
using RocketInsights.Contextual.Models;
using RocketInsights.DXP.Services;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Kontent.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private static IServiceCollection ServiceCollection => new ServiceCollection().AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace));

        public IntegrationTests()
        {
            ServiceCollection.AddHttpContextAccessor();
            ServiceCollection.AddContextual();

            ServiceCollection.AddDXP();
            ServiceCollection.AddKontent();
        }

        [TestMethod]
        public async Task TestRetrievingAContentFragmentFromKontent()
        {
            var provider = ServiceCollection
                .AddHttpContextAccessor()
                .AddContextual()
                .AddSingleton<IContextStore, DefaultContextStore>()
                .AddDXP()
                .AddKontent()
                .BuildServiceProvider();

            var contextStore = provider.GetRequiredService<IContextStore>();

            contextStore.Set("context", new Context()
            {
                Culture = new CultureInfo("en-US"),
                Identity = new ClaimsIdentity(),
                Content = new Content()
            });
                
            var experienceService = provider.GetRequiredService<IExperienceService>();

            var fragment = await experienceService.GetFragmentAsync("about_us_f869f8f");

            Assert.IsNotNull(fragment);
        }

        [TestMethod]
        public async Task TestRetrievingACompositionFromKontent()
        {
            var provider = ServiceCollection
                .AddHttpContextAccessor()
                .AddContextual()
                .AddSingleton<IContextStore, DefaultContextStore>()
                .AddDXP()
                .AddKontent()
                .BuildServiceProvider();

            var contextStore = provider.GetRequiredService<IContextStore>();

            contextStore.Set("context", new Context()
            {
                Culture = new CultureInfo("en-US"),
                Identity = new ClaimsIdentity(),
                Content = new Content
                {
                    { "uri", "about-us" }
                }
            });

            var experienceService = provider.GetRequiredService<IExperienceService>();

            var composition = await experienceService.GetCompositionAsync();

            Assert.IsNotNull(composition);
        }
    }
}
