using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Factories;
using System.Globalization;
using System.Security.Claims;

namespace RocketInsights.Contextual.Tests
{
    [TestClass]
    public class ModelTests
    {
        [TestMethod]
        public void FactoryCreateContext()
        {
            var mockCultureFactory = new Mock<IFactory<CultureInfo>>();
            mockCultureFactory.Setup(f => f.Create()).Returns(new CultureInfo("en-US"));

            var mockIdentityFactory = new Mock<IFactory<ClaimsIdentity>>();
            mockIdentityFactory.Setup(f => f.Create()).Returns(new ClaimsIdentity());

            var factory = new ContextFactory(mockCultureFactory.Object, mockIdentityFactory.Object);

            var context = factory.Create();

            Assert.AreEqual("en-US", context.Culture.Name);
            Assert.IsTrue(context.Identity.IsAuthenticated);
        }
    }
}
