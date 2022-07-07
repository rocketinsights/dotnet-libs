using RocketInsights.Common.Patterns;
using RocketInsights.Contextual.Models;

namespace RocketInsights.Contextual.Services
{
    public class ContextService : IContextService
    {
        private IContextStore Store { get; }
        private IFactory<Context> ContextFactory { get; }

        public ContextService(IContextStore store, IFactory<Context> contextFactory)
        {
            Store = store;
            ContextFactory = contextFactory;
        }

        public void GenerateAndSetContext()
        {
            var context = ContextFactory.Create();

            Store.Set("context", context);
        }

        public bool TryGetContext(out Context context)
        {
            return Store.TryGet("context", out context);
        }
    }
}