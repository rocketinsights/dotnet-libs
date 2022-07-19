using RocketInsights.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace RocketInsights.Common.Extensions
{
    public static class ContentExtensions
    {
        private static JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static bool TryParse<T>(this Content content, string key, out T parsed)
        {
            parsed = default;

            try
            {
                if (content.TryGetValue(key, out object obj))
                {
                    var json = JsonSerializer.Serialize(obj, JsonOptions);

                    parsed = JsonSerializer.Deserialize<T>(json, JsonOptions);

                    return true;
                }
            }
            catch(Exception)
            {
                
            }

            return false;
        }
    }
}
