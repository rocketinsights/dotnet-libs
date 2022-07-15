using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class CustomElement : ElementBase
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
