using System.Collections.Generic;

namespace RocketInsights.Contextual
{
    public interface IContextStore
    {
        void Set<T>(string key, T value) where T : class;
        bool TryGet<T>(string key, out T value) where T : class;
    }

    public class DefaultContextStore : IContextStore
    {
        private Dictionary<string, object> Store { get; }

        public DefaultContextStore()
        {
            Store = new Dictionary<string, object>();
        }

        public void Set<T>(string key, T value) where T : class
        {
            Store.Add(key, value);
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            if (Store.TryGetValue(key, out var obj))
            {
                if (obj is T)
                {
                    value = obj as T;

                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}