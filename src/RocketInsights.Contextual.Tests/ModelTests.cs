using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RocketInsights.Common.Models;
using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Factories;
using RocketInsights.Contextual.Models;
using System;
using System.Collections.Generic;
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

            var mockContentFactory = new Mock<IFactory<Content>>();
            mockContentFactory.Setup(f => f.Create()).Returns(Task.FromResult(new Content()));

            var factory = new ContextFactory(mockCultureFactory.Object, mockIdentityFactory.Object, mockContentFactory.Object);

            var context = await factory.Create();

            Assert.AreEqual("en-US", context.Culture.Name);
            Assert.IsFalse(context.Identity.IsAuthenticated);
        }

        [TestMethod]
        public async Task RequestContextUriJsonDeserializationTest()
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

        [TestMethod]
        public async Task ContentUriJsonDeserializationTest()
        {
            var expected = new Uri("https://www.example.com/hello/world");

            var json = @"{
                ""uri"": ""https://www.example.com/hello/world""
            }";

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var context = JsonSerializer.Deserialize<Content>(json, options);

            if(context.TryGetValue("uri", out object obj))
            {
                var uriJson = JsonSerializer.Serialize(obj, options);

                var uri = JsonSerializer.Deserialize<Uri>(uriJson, options);

                Assert.AreEqual(expected.AbsolutePath, uri.AbsolutePath);
            }
        }
    }
}
