using RocketInsights.DXP.Providers.Kontent.Common;
using System.Collections.Generic;
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

        public static Dictionary<string, object> DeserializeElements(this JsonElement? elements)
        {
            var deserializedElements = new Dictionary<string, object>();
            if (elements == null)
                return deserializedElements;

            var elementObjects = JsonSerializer.Deserialize<Dictionary<string, object>>((JsonElement)elements);
            if (elementObjects == null)
                return deserializedElements;

            foreach (var elementObject in elementObjects)
            {
                var jsonElement = JsonDocument.Parse(JsonSerializer.Serialize(elementObject.Value)).RootElement;
                deserializedElements.Add(elementObject.Key, GetElementObject(jsonElement));
            }

            return deserializedElements;
        }

        private static object GetElementObject(JsonElement jsonElement)
        {
            var response = new Dictionary<string, object>();

            switch (jsonElement.GetProperty("type").GetString())
            {
                case ElementTypeConstants.Asset:
                case ElementTypeConstants.Custom:
                case ElementTypeConstants.DateTime:
                case ElementTypeConstants.ModularContent:
                case ElementTypeConstants.MultipleChoice:
                case ElementTypeConstants.Number:
                case ElementTypeConstants.Text:
                case ElementTypeConstants.UrlSlug:
                    response.Add("value", jsonElement.GetSubProperty("value") ?? new JsonElement());
                    break;

                case ElementTypeConstants.RichText:
                    response.Add("images", jsonElement.GetSubProperty("images") ?? new JsonElement());
                    response.Add("links", jsonElement.GetSubProperty("links") ?? new JsonElement());
                    response.Add("modular_content", jsonElement.GetSubProperty("modular_content") ?? new JsonElement());
                    response.Add("value", jsonElement.GetSubProperty("value") ?? new JsonElement());
                    break;

                case ElementTypeConstants.Taxonomy:
                    response.Add("taxonomy_group", jsonElement.GetSubProperty("taxonomy_group") ?? new JsonElement());
                    response.Add("value", jsonElement.GetSubProperty("value") ?? new JsonElement());
                    break;
            }

            return response;
        }
    }
}
