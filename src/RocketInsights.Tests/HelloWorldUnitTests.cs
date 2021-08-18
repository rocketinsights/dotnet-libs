using Microsoft.VisualStudio.TestTools.UnitTesting;
using RocketInsights.Common;

namespace RocketInsights.Tests
{
    [TestClass]
    public class HelloWorldUnitTests
    {
        [TestMethod]
        public void Concatenation()
        {
            var model = new HelloWorld
            {
                Foo = "foo",
                Bar = "bar"
            };

            var concatenated = model.Concatenate();

            Assert.AreEqual("foobar", concatenated);
        }

        [TestMethod]
        public void FailingTest()
        {
            Assert.IsTrue(false);
        }
    }
}
