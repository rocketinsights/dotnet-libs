using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class DateTimeElement : ElementBase
    {
        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }
}
