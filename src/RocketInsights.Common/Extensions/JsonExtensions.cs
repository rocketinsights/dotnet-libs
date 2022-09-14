using RocketInsights.Common.Json.Converters;
using System.Text;
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

        public static byte[] ToByteAray<T>(this T source)
        {
            return JsonSerializer.SerializeToUtf8Bytes(source);
        }

        public static T Deserialize<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }

        public static T Deserialize<T>(this byte[] source)
        {
            var sourceString = Encoding.UTF8.GetString(source);
            return JsonSerializer.Deserialize<T>(sourceString);
        }
    }
}
