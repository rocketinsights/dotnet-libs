using Microsoft.AspNetCore.Http;
using RocketInsights.Contextual.Services;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.AspNetCore.Middleware
{
    public class ContextualMiddleware
    {
        private readonly RequestDelegate _next;

        public ContextualMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IContextService contextService)
        {
            await contextService.GenerateAndSetContext();

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}