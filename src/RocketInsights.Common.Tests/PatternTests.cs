using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RocketInsights.Common.Patterns.Pipelines;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.Common.Tests
{
    public class ConcatenationOperation : IChainableOperation<string>
    {
        public Task<string> InvokeAsync(string input)
        {
            var output = string.Concat(input, input);

            return Task.FromResult(output);
        }
    }

    public class ConditionalConcatenationOperation : ConditionalOperation<string>
    {
        protected override Func<string, bool> Condition => (input) => input.Length < 12;

        protected override Task<string> ConditionalInvokeAsync(string input)
        {
            var output = string.Concat(input, input);

            return Task.FromResult(output);
        }
    }

    public class ToAscii : IOperation<char, int>
    {
        public Task<int> InvokeAsync(char input)
        {
            return Task.FromResult((int)input);
        }
    }

    [TestClass]
    public class PatternTests
    {
        private static IServiceCollection ServiceCollection => new ServiceCollection().AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace).AddDebug());

        public PatternTests()
        {

        }

        [TestMethod]
        public async Task TestSimplePipeline()
        {
            var stages = new List<IChainableOperation<string>>() { new ConcatenationOperation(), new ConcatenationOperation(), new ConcatenationOperation() };

            var pipeline = new AggregatePipeline<string>(stages);

            var result1 = await pipeline.InvokeAsync("a");
            var result2 = await pipeline.InvokeAsync("ab");

            Assert.AreEqual("aaaaaaaa", result1);
            Assert.AreEqual("abababababababab", result2);
        }

        [TestMethod]
        public async Task TestPipelineWithConditionalOperation()
        {
            var stages = new List<IChainableOperation<string>>() { new ConditionalConcatenationOperation(), new ConditionalConcatenationOperation(), new ConditionalConcatenationOperation() };

            var pipeline = new AggregatePipeline<string>(stages);

            var result1 = await pipeline.InvokeAsync("ab");
            var result2 = await pipeline.InvokeAsync("abc");

            Assert.AreEqual("abababababababab", result1);
            Assert.AreEqual("abcabcabcabc", result2); // The third operation is not invoked because the predicate evaluates as false
        }
    }
}