using RocketInsights.Common.Patterns;
using System.Threading.Tasks;

namespace RocketInsights.DXP.Factories
{
    // This is a good candidate to be moved into common. It isn't DXP specific
    public class SingleDependencyFactory<T> : IFactory<T>
    {
        private T Service { get; }

        public SingleDependencyFactory(T service)
        {
            Service = service;
        }

        public Task<T> Create()
        {
            return Task.FromResult(Service);
        }
    }
}
