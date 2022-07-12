using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns.Pipelines
{
    public interface IOperation<TInput, TOutput>
    {
        Task<TOutput> InvokeAsync(TInput input);
    }
}