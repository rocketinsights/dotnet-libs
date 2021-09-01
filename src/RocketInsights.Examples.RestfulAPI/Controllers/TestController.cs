using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RocketInsights.Contextual.Services;
using RocketInsights.Examples.RestfulAPI.Constants;
using System;

namespace RocketInsights.Examples.RestfulAPI.Controllers
{
    [Route("")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private IContextService ContextService { get; }

        public TestController(IContextService contextService)
        {
            ContextService = contextService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(DateTime.Now.ToString("D"));
        }

        [HttpGet("admin")]
        [Authorize(Roles.Administrator)]
        public IActionResult Admin()
        {
            return Ok();
        }

        [HttpGet("context")]
        public IActionResult Context()
        {
            if(ContextService.TryGetContext(out var context))
            {
                return Ok(context.Culture.Name);
            }

            return NotFound();
        }
    }
}