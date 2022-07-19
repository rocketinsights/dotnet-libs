using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class RichTextElement : ElementBase
    {
        [JsonPropertyName("images")]
        public Dictionary<string, ElementImage>? Images { get; set; }

        [JsonPropertyName("links")]
        public Dictionary<string, ElementLink>? Links { get; set; }

        [JsonPropertyName("modular_content")]
        public List<string>? ModularContent { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
