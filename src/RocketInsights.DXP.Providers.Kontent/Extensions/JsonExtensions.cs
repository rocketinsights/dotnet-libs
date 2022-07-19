using System.Text.Json;

namespace RocketInsights.DXP.Providers.Kontent.Extensions
{
    public static class JsonExtensions
    {
        public static JsonElement? GetSubProperty(this JsonElement jsonElement, string property)
        {
            var properties = property.Split(".");
            var currentElement = jsonElement;
            foreach (var propertyName in properties)
            {
                if (currentElement.TryGetProperty(propertyName, out var element))
                    currentElement = element;
                else
                    return null;
            }

            return currentElement;
        }
    }
}
