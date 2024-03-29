﻿using RocketInsights.Common.Models;
using System.Globalization;
using System.Security.Claims;

namespace RocketInsights.Contextual.Models
{
    public class Context
    {
        public CultureInfo Culture { get; set; }
        public ClaimsIdentity Identity { get; set; }
        public Content Content { get; set; }
    }
}