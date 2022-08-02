using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RocketInsights.Common.Json.Converters
{
    public class ObjectToPrimitiveConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.True)
            {
                return true;
            }

            if (reader.TokenType == JsonTokenType.False)
            {
                return false;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetDouble();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                if (reader.TryGetDateTime(out DateTime datetime))
                {
                    // If an offset was provided, use DateTimeOffset.
                    if (datetime.Kind == DateTimeKind.Local)
                    {
                        if (reader.TryGetDateTimeOffset(out DateTimeOffset datetimeOffset))
                        {
                            return datetimeOffset;
                        }
                    }

                    return datetime;
                }

                return reader.GetString();
            }

            // Use JsonElement as fallback.
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                var element = document.RootElement.Clone();

                if (element.ValueKind == JsonValueKind.Array)
                {
                    var collection = new List<object>();

                    using (var enumerator = element.EnumerateArray())
                    {
                        foreach (var current in enumerator)
                        {
                            var json = current.GetRawText();

                            collection.Add(JsonSerializer.Deserialize(json, typeof(object), options));
                        }

                    }

                    return collection;
                }

                if (element.ValueKind == JsonValueKind.Object)
                {
                    var dictionary = new Dictionary<string, object>();

                    using (var enumerator = element.EnumerateObject())
                    {
                        foreach (var current in enumerator)
                        {
                            var json = current.Value.GetRawText();

                            dictionary.Add(current.Name, JsonSerializer.Deserialize(json, typeof(object), options));
                        }
                    }

                    return dictionary;
                }

                return element;
            }
        }

        public override void Write(Utf8JsonWriter writer, object objectToWrite, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
    }
}
