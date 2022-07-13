using RocketInsights.Common.Patterns;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.AspNetCore.Factories
{
    public class ThreadCultureFactory : IFactory<CultureInfo>
    {
        public Task<CultureInfo> Create()
        {
            return Task.FromResult(Thread.CurrentThread.CurrentCulture);
        }
    }
}
