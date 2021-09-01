namespace RocketInsights.Contextual
{
    public interface IContextStore
    {
        void Set<T>(string key, T value) where T : class;
        bool TryGet<T>(string key, out T value) where T : class;
    }
}