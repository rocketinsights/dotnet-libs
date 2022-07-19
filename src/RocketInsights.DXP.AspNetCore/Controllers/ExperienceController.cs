using Microsoft.AspNetCore.Mvc;
using RocketInsights.DXP.Models;
using RocketInsights.DXP.Services;
using System.Threading.Tasks;

namespace RocketInsights.DXP.AspNetCore.Controllers
{
    [Route("experience")]
    [ApiController]
    public class ExperienceController : ControllerBase
    {
        protected IExperienceService ExperienceService { get; }

        public ExperienceController(IExperienceService experienceService)
        {
            ExperienceService = experienceService;
        }

        [HttpPost("composition")]
        public async Task<Composition> GetComposition()
        {
            return await ExperienceService.GetCompositionAsync();
        }

        [HttpGet("fragment/{id}")]
        public async Task<Fragment> GetFragment(string id)
        {
            return await ExperienceService.GetFragmentAsync(id);
        }
    }
}