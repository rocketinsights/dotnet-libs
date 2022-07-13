using Microsoft.AspNetCore.Mvc;
using RocketInsights.Contextual;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;
using System.Threading.Tasks;

namespace RocketInsights.DXP.AspNetCore.Controllers
{
    [Route("experience")]
    [ApiController]
    public class ExperienceController : ControllerBase
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

        [HttpGet("composition")]
        public async Task<Composition> GetComposition()
        {
            return await LayoutService.GetCompositionAsync();
        }

        [HttpGet("fragment/{id}")]
        public async Task<Fragment> GetFragment(string id)
        {
            return await ContentService.GetFragmentAsync(id);
        }
    }
}