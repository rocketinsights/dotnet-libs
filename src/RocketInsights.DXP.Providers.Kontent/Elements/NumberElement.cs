using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class NumberElement : ElementBase
    {
        [JsonPropertyName("value")]
        public decimal? Value { get; set; }
    }
}
