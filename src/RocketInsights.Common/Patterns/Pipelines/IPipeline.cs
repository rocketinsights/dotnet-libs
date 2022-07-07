using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns.Pipelines
{
    public interface IPipeline<T>
    {
        IEnumerable<IOperation<T>> Stages { get; }

        Task<T> RunAsync(T input);
    }
}
