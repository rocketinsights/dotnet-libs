using System;
using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns.Pipelines
{
    public abstract class ConditionalOperation<T> : IOperation<T>
    {
        protected abstract Func<T, bool> Predicate { get; }

        public async Task<T> InvokeAsync(T input)
        {
            return Predicate.Invoke(input) ? await ConditionalInvoke(input) : input;
        }

        protected abstract Task<T> ConditionalInvoke(T input);
    }
}
