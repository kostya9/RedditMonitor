using KPI.RedditMonitor.Collector.RedditPull;
using KPI.RedditMonitor.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubredditsController : ControllerBase
    {
        private readonly ImagePostsRepository _service;

        public SubredditsController(ImagePostsRepository service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<string>>> Find()
        {
            var resp = await _service.GetAllSubreddits();

            if(resp == null)
                return NotFound();

            return resp;
        }
    }
}
