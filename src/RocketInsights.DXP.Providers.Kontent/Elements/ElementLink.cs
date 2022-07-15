using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class ElementLink
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("codename")]
        public string? Codename { get; set; }

        [JsonPropertyName("url_slug")]
        public string? UrlSlug { get; set; }
    }
}
