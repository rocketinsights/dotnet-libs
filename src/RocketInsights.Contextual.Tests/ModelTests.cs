using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Factories;
using RocketInsights.DXP.Services;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.Tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public async Task FactoryCreateContext()
        {
            var mockCultureFactory = new Mock<IFactory<CultureInfo>>();
            mockCultureFactory.Setup(f => f.Create()).Returns(Task.FromResult(new CultureInfo("en-US")));

            var mockIdentityFactory = new Mock<IFactory<ClaimsIdentity>>();
            mockIdentityFactory.Setup(f => f.Create()).Returns(Task.FromResult(new ClaimsIdentity()));

            var factory = new ContextFactory(mockCultureFactory.Object, mockIdentityFactory.Object);

            var context = await factory.Create();

            Assert.AreEqual("en-US", context.Culture.Name);
            Assert.IsFalse(context.Identity.IsAuthenticated);
        }
    }
}
