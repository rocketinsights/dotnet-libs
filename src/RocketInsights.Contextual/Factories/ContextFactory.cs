using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Models;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.Factories
{
    public class ContextFactory : IFactory<Context>
    {
        private IFactory<CultureInfo> CultureFactory { get; }
        private IFactory<ClaimsIdentity> IdentityFactory { get; }
        private IFactory<RequestContext> RequestFactory { get; }

        public ContextFactory(IFactory<CultureInfo> cultureFactory, IFactory<ClaimsIdentity> identityFactory, IFactory<RequestContext> requestFactory)
        {
            CultureFactory = cultureFactory;
            IdentityFactory = identityFactory;
            RequestFactory = requestFactory;
        }

        public async Task<Context> Create()
        {
            var context = new Context
            {
                Culture = await CultureFactory.Create(),
                Identity = await IdentityFactory.Create(),
                Request = await RequestFactory.Create()
            };

            return context;
        }
    }
}
