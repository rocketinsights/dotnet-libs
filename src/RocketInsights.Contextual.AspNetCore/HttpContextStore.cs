using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RocketInsights.Contextual.AspNetCore
{
    public class HttpContextStore : IContextStore
    {
        private IHttpContextAccessor Accessor { get; }

        public HttpContextStore(IHttpContextAccessor accessor)
        {
            Accessor = accessor;
        }

        public void Set<T>(string key, T value) where T : class
        {
            Accessor.HttpContext.Items.Add(key, value);
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            if(Accessor.HttpContext.Items.TryGetValue(key, out var obj))
            {
                if(obj is T)
                {
                    value = obj as T;

                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}
