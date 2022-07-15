using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class TaxonomyElement : ElementBase
    {
        [JsonPropertyName("taxonomy_group")]
        public string? TaxonomyGroup { get; set; }

        [JsonPropertyName("value")]
        public List<TaxonomyValue>? Value { get; set; }
    }

    public class TaxonomyValue
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("codename")]
        public string? Codename { get; set; }
    }
}
