using Microsoft.VisualStudio.TestTools.UnitTesting;
using RocketInsights.Common.Patterns.Pipelines;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.Common.Tests
{
    public class ConcatenationOperation : IOperation<string>
    {
        public Task<string> InvokeAsync(string input)
        {
            var output = string.Concat(input, input);

            return Task.FromResult(output);
        }
    }

    public class ConditionalConcatenationOperation : ConditionalOperation<string>
    {
        protected override Func<string, bool> Predicate => (input) => input.Length < 12;

        protected override Task<string> ConditionalInvoke(string input)
        {
            var output = string.Concat(input, input);

            return Task.FromResult(output);
        }
    }

    [TestClass]
    public class PatternTests
    {
        public PatternTests()
        {

        }

        [TestMethod]
        public async Task TestSimplePipeline()
        {
            var stages = new List<IOperation<string>>() { new ConcatenationOperation(), new ConcatenationOperation(), new ConcatenationOperation() };

            var pipeline = new AggregatePipeline<string>(stages);

            var result1 = await pipeline.RunAsync("a");
            var result2 = await pipeline.RunAsync("ab");

            Assert.AreEqual("aaaaaaaa", result1);
            Assert.AreEqual("abababababababab", result2);
        }

        [TestMethod]
        public async Task TestPipelineWithConditionalOperation()
        {
            var stages = new List<IOperation<string>>() { new ConditionalConcatenationOperation(), new ConditionalConcatenationOperation(), new ConditionalConcatenationOperation() };

            var pipeline = new AggregatePipeline<string>(stages);

            var result1 = await pipeline.RunAsync("ab");
            var result2 = await pipeline.RunAsync("abc");

            Assert.AreEqual("abababababababab", result1);
            Assert.AreEqual("abcabcabcabc", result2); // The third operation is not invoked because the predicate evaluates as false
        }
    }
}