using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Models;
using System.Globalization;
using System.Security.Claims;

namespace RocketInsights.Contextual.Factories
{
    public class ContextFactory : IFactory<Context>
    {
        private IFactory<CultureInfo> CultureFactory { get; }
        private IFactory<ClaimsIdentity> IdentityFactory { get; }

        public ContextFactory(IFactory<CultureInfo> cultureFactory, IFactory<ClaimsIdentity> identityFactory)
        {
            CultureFactory = cultureFactory;
            IdentityFactory = identityFactory;
        }

        public Context Create()
        {
            var context = new Context
            {
                Culture = CultureFactory.Create(),
                Identity = IdentityFactory.Create()
            };

            return context;
        }
    }
}
