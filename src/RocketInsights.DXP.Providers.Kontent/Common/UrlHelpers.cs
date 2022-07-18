namespace RocketInsights.DXP.Providers.Kontent.Common
{
    public static class UrlHelpers
    {
        public static string GetKontentBaseUrl(string id)
        {
            return $"https://deliver.kontent.ai/{id}";
        }
    }
}
