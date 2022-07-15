﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RocketInsights.DXP.Providers.Kontent.Elements
{
    public class SubpagesElement : ElementBase
    {
        [JsonPropertyName("value")]
        public List<string>? Value { get; set; }
    }
}
