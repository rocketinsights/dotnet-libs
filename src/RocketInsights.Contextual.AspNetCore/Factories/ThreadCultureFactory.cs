using RocketInsights.Common.Patterns;
using System.Globalization;
using System.Threading;

namespace RocketInsights.Contextual.AspNetCore.Factories
{
    public class ThreadCultureFactory : IFactory<CultureInfo>
    {
        public CultureInfo Create()
        {
            return Thread.CurrentThread.CurrentCulture;
        }
    }
}
