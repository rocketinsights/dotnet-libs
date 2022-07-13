using System.Threading.Tasks;

namespace RocketInsights.Common.Patterns
{
    public interface IFactory<T>
    {
        Task<T> Create();
    }
}
