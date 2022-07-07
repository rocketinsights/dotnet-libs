using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns.Pipelines
{
    public class AggregatePipeline<T> : IPipeline<T>
    {
        public IEnumerable<IOperation<T>> Stages { get; }

        public AggregatePipeline(IEnumerable<IOperation<T>> operations)
        {
            Stages = operations;
        }

        public async Task<T> RunAsync(T input)
        {
            foreach(var stage in Stages)
            {
                input = await stage.InvokeAsync(input);
            }

            return input;
        }
    }
}
