using Microsoft.AspNetCore.Http;
using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Models;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.AspNetCore.Factories
{
    public class HttpBodyRequestFactory : IFactory<RequestContext>
    {
        private IHttpContextAccessor Accessor { get; }

        public HttpBodyRequestFactory(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public async Task<RequestContext> Create()
        {
            //var body = await ReadBodyAsync(Accessor.HttpContext.Request);

            var requestContext = await JsonSerializer.DeserializeAsync<RequestContext>(Accessor.HttpContext.Request.Body);

            return requestContext;
        }

        private async Task<string> ReadBodyAsync(HttpRequest req)
        {
            if (!req.Body.CanSeek)
            {
                //req.EnableBuffering();
            }

            req.Body.Position = 0;

            var reader = new StreamReader(req.Body, Encoding.UTF8);

            var body = await reader.ReadToEndAsync().ConfigureAwait(false);

            req.Body.Position = 0;

            return body;
        }
    }
}
