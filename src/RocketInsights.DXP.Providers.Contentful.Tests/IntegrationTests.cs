
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RocketInsights.Contextual;
using RocketInsights.Contextual.Models;
using RocketInsights.DXP.Providers.Contentful.Extensions;
using RocketInsights.DXP.Services;
using System.Globalization;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Providers.Contentful.Tests
{
    [TestClass]
    public class IntegrationTests
    {
        private static IServiceCollection ServiceCollection { get; set; }

        public IntegrationTests()
        {
            IConfigurationRoot configurationRoot = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            ServiceCollection = new ServiceCollection()
               .AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace))
               .AddHttpContextAccessor()
               .AddContextual()
               .AddSingleton<IContextStore, DefaultContextStore>()
               .AddDXP()
               .AddContentful(configurationRoot);
               
        }

        [TestMethod]
        public async Task TestRetrievingAContentFragmentFromContentful()
        {
            var provider = ServiceCollection.BuildServiceProvider();

            var contextStore = provider.GetRequiredService<IContextStore>();
            contextStore.Set("context", new Context()
            {
                Culture = new CultureInfo("en-US"),
                Identity = new ClaimsIdentity()
            });

            var experienceService = provider.GetRequiredService<IExperienceService>();

            var fragment = await experienceService.GetFragmentAsync("3EK9sDbFFm69x1azZdd6MJ");
            Assert.AreEqual("3EK9sDbFFm69x1azZdd6MJ", fragment.Id);
        }
    }
}
