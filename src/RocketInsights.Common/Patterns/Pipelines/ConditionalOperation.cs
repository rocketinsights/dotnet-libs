using System;
using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns.Pipelines
{
    public abstract class ConditionalOperation<T> : IChainableOperation<T>
    {
        protected abstract Func<T, bool> Condition { get; }

        public async Task<T> InvokeAsync(T input)
        {
            return Condition.Invoke(input) ? await ConditionalInvokeAsync(input) : input;
        }

        protected abstract Task<T> ConditionalInvokeAsync(T input);
    }
}
