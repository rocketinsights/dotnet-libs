using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class ModularContent : ElementBase
    {
        [JsonPropertyName("value")]
        public List<string>? Value { get; set; }
    }
}
