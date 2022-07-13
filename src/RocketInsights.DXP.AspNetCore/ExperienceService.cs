using RocketInsights.Contextual;
using RocketInsights.Contextual.Models;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RocketInsights.DXP.AspNetCore
{
    [RoutePrefix("experience")]
    public class ExperienceController : Controller
    {
        protected IContextStore ContextStore { get; }
        protected ILayoutService LayoutService { get; }
        protected IContentService ContentService { get; }

        public ExperienceController(IContextStore contextStore, ILayoutService layoutService, IContentService contentService)
        {
            ContextStore = contextStore;
            LayoutService = layoutService;
            ContentService = contentService;
        }

        [HttpGet, Route("composition")]
        public async Task<Composition> GetComposition()
        {
            return await LayoutService.GetCompositionAsync();
        }

        [HttpGet, Route("Fragment/{id}")]
        public async Task<Fragment> GetFragment(string id)
        {
            return await ContentService.GetFragmentAsync(id);
        }
    }
}