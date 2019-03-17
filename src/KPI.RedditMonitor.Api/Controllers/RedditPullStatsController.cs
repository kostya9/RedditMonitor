using KPI.RedditMonitor.Collector.RedditPull;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedditPullStatsController : ControllerBase
    {
        private readonly RedditPullStats _stats;

        public RedditPullStatsController(RedditPullStats stats)
        {
            _stats = stats;
        }

        [HttpGet]
        public async Task<ActionResult<RedditPullStats>> Get()
        {
            return _stats;
        }
    }
}
