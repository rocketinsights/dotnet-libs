using Microsoft.VisualStudio.TestTools.UnitTesting;
using RocketInsights.Common.Models;
using RocketInsights.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace RocketInsights.Common.Tests
{
    [TestClass]
    public class JsonTests
    {
        public JsonTests()
        {

        }

        [TestMethod]
        public void TestConverter()
        {
            var content = new Content()
            {
                { "string", "Hello World!" },
                { "uri", new Uri("https://www.example.com/directory/file.extension") },
                { "dateTime", DateTime.Now },
                { "boolean", true },
                { "array", new List<string>() { "a", "b", "c" } }
            };

            var json = content.Serialize();

            var deserialized = json.Deserialize<Content>();

            content.Add("object", deserialized);

            json = content.Serialize();

            deserialized = json.Deserialize<Content>();

            Assert.IsNotInstanceOfType(deserialized["string"], typeof(JsonElement));
        }
    }
}