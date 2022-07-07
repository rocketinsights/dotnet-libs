using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns.Pipelines
{
    public interface IOperation<T>
    {
        Task<T> InvokeAsync(T input);
    }
}
