using RocketInsights.Common.Json.Converters;
using System.Text.Json;

namespace RocketInsights.Common.Extensions
{
    public static class JsonExtensions
    {
        private static JsonSerializerOptions JsonOptions => new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters =
            {
                new ObjectToPrimitiveConverter()
            }
        };

        public static string Serialize(this object obj)
        {
            return JsonSerializer.Serialize(obj, JsonOptions);
        }

        public static T Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
    }
}
