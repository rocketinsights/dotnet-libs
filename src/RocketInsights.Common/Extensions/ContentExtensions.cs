using RocketInsights.Common.Models;
using System;

namespace RocketInsights.Common.Extensions
{
    public static class ContentExtensions
    {
        public static bool TryParse<T>(this Content content, string key, out T parsed)
        {
            parsed = default;

            try
            {
                if (content.TryGetValue(key, out object obj))
                {
                    var json = obj.Serialize();

                    parsed = json.Deserialize<T>();

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
