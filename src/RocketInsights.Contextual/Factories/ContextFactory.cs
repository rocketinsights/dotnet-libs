using RocketInsights.Common.Models;
using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Models;
using System;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.Factories
{
    public class ContextFactory : IFactory<Context>
    {
        private IFactory<CultureInfo> CultureFactory { get; }
        private IFactory<ClaimsIdentity> IdentityFactory { get; }
        private IFactory<Content> ContentFactory { get; }

        public ContextFactory(IFactory<CultureInfo> cultureFactory, IFactory<ClaimsIdentity> identityFactory, IFactory<Content> contentFactory)
        {
            CultureFactory = cultureFactory;
            IdentityFactory = identityFactory;
            ContentFactory = contentFactory;
        }

        public async Task<Context> Create()
        {
            return new Context()
            {
                Culture = await CultureFactory.Create(),
                Identity = await IdentityFactory.Create(),
                Content = await ContentFactory.Create()
            };
        }
    }
}
