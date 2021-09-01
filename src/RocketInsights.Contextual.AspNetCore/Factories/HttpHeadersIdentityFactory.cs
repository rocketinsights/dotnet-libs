using Microsoft.AspNetCore.Http;
using RocketInsights.Common.Patterns;
using System.Security.Claims;

namespace RocketInsights.Contextual.AspNetCore.Factories
{
    public class HttpHeadersIdentityFactory : IFactory<ClaimsIdentity>
    {
        private IHttpContextAccessor Accessor { get; }

        public HttpHeadersIdentityFactory(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public ClaimsIdentity Create()
        {
            var identity = Accessor.HttpContext.User.Identity;

            if (identity.IsAuthenticated)
            {
                return new ClaimsIdentity(identity);
            }

            return new ClaimsIdentity();
        }
    }
}
