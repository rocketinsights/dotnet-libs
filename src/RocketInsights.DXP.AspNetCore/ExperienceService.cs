using RocketInsights.Contextual;
using RocketInsights.Contextual.Models;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;

namespace RocketInsights.DXP.AspNetCore
{
    public class ExperienceService
    {
        protected IContextStore ContextStore { get; }
        protected ILayoutService LayoutService { get; }
        protected IContentService ContentService { get; }

        public ExperienceService(IContextStore contextStore, ILayoutService layoutService, IContentService contentService)
        {
            ContextStore = contextStore;
            LayoutService = layoutService;
            ContentService = contentService;
        }

        public async Task<Composition> GetComposition()
        {
            if (ContextStore.TryGet<Context>("context", out var context))
            {
                // You could do stuff with this if needed
            }

            return await LayoutService.GetComposition();
        }
    }
}