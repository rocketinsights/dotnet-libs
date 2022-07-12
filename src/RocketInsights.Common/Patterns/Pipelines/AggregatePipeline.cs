using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns.Pipelines
{
    public class AggregatePipeline<T> : IChainableOperation<T>
    {
        public IEnumerable<IChainableOperation<T>> Operations { get; }

        public AggregatePipeline(IEnumerable<IChainableOperation<T>> operations)
        {
            Operations = operations;
        }

        public async Task<T> InvokeAsync(T input)
        {
            foreach(var operation in Operations)
            {
                input = await operation.InvokeAsync(input);
            }

            return input;
        }
    }
}
