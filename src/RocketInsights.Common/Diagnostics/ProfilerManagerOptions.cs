using Microsoft.Extensions.Logging;
using System;

namespace RocketInsights.Common.Diagnostics
{
    public class ProfilerManagerOptions
    {
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Configure what happens on completion (defaults to trace logging name and duration)
        /// </summary>
        public Action<string, TimeSpan, ProfilerManager> Completion { get; set; } = (name, duration, manager) => manager.Logger.LogTrace($"{name} took {duration.TotalMilliseconds}ms");
    }
}
