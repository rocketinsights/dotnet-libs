using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Factories;
using RocketInsights.Contextual.Models;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;
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

            var mockRequestFactory = new Mock<IFactory<RequestContext>>();
            mockRequestFactory.Setup(f => f.Create()).Returns(Task.FromResult(new RequestContext()));

            var factory = new ContextFactory(mockCultureFactory.Object, mockIdentityFactory.Object, mockRequestFactory.Object);

            var context = await factory.Create();

            Assert.AreEqual("en-US", context.Culture.Name);
            Assert.IsFalse(context.Identity.IsAuthenticated);
        }

        [TestMethod]
        public async Task RequestContextJsonDeserializationTest()
        {
            var expected = new Uri("https://www.example.com/hello/world", UriKind.RelativeOrAbsolute);

            var json = @"{
                ""uri"": ""https://www.example.com/hello/world""
            }";

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var requestContext = JsonSerializer.Deserialize<RequestContext>(json, options);

            Assert.AreEqual(expected.AbsolutePath, requestContext.Uri.AbsolutePath);
        }
    }
}
