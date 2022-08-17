using System.Collections.Generic;

namespace RocketInsights.DXP.Providers.Kontent.Extensions
{
    public static class TypeExtensions
    {
        public static object? GetValueAsObject(this Dictionary<string, object>? dict, string key)
        {
            return dict != null && dict.TryGetValue(key, out var val)
                ? val
                : null;
        }

        public static Dictionary<string, object>? GetValueAsDictionary(this Dictionary<string, object>? dict, string key)
        {
            return dict != null && dict.TryGetValue(key, out var val)
                ? val as Dictionary<string, object>
                : null;
        }
    }
}
