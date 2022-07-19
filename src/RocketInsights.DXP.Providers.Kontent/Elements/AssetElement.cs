using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class AssetElement : ElementBase
    {
        [JsonPropertyName("value")]
        public List<AssetValue>? Value { get; set; }
    }

    public class AssetValue
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("size")]
        public long? Size { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("renditions")]
        public Dictionary<string, AssetRendition>? Renditions { get; set; }
    }

    public class AssetRendition
    {
        [JsonPropertyName("rendition_id")]
        public string? RenditionId { get; set; }

        [JsonPropertyName("preset_id")]
        public string? PresetId { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("query")]
        public string? Query { get; set; }
    }
}
