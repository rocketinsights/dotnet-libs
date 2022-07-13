using Microsoft.AspNetCore.Http;
using RocketInsights.Common.Patterns;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.AspNetCore.Factories
{
    public class HttpHeadersIdentityFactory : IFactory<ClaimsIdentity>
    {
        private IHttpContextAccessor Accessor { get; }

        public HttpHeadersIdentityFactory(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public Task<ClaimsIdentity> Create()
        {
            var identity = Accessor.HttpContext.User.Identity;
            var claimsIdentity = identity.IsAuthenticated ? new ClaimsIdentity(identity) : new ClaimsIdentity();

            return Task.FromResult(claimsIdentity);
        }
    }
}
