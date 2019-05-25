using KPI.RedditMonitor.Collector.RedditPull;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedditDataController : ControllerBase
    {
        private readonly RedditPostsService _service;

        public RedditDataController(RedditPostsService service)
        {
            _service = service;
        }

        [HttpPost("findPost")]
        public async Task<ActionResult<RedditPost>> Find(PostFindRequest request)
        {
            var resp = await _service.GetPost(request.Url);

            if(resp == null)
                return NotFound();

            return resp;
        }
    }

    public class PostFindRequest
    {
        [Required]
        public string Url { get; set; }
    }
}
