using Microsoft.AspNetCore.Http;
using RocketInsights.Common.Models;
using RocketInsights.Common.Patterns;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RocketInsights.Contextual.AspNetCore.Factories
{
    public class HttpBodyContentFactory : IFactory<Content>
    {
        private IHttpContextAccessor Accessor { get; }

        public HttpBodyContentFactory(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public async Task<Content> Create()
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<Content>(Accessor.HttpContext.Request.Body);
            }
            catch(Exception)
            {
                return new Content();
            }
        }
    }
}
