using KPI.RedditMonitor.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KPI.RedditMonitor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopImagesController : ControllerBase
    {
        private readonly TopImageDbAdapter _topImages;

        public TopImagesController(TopImageDbAdapter topImages)
        {
            _topImages = topImages;
        }

        [HttpGet]
        public async Task<ActionResult<TopImageResponse>> Get([FromQuery]bool ignored = false, [FromQuery] string[] subreddits = null)
        {
            var images = await _topImages.GetTop(!ignored ? 12 : 100, ignored, subreddits);
            var count = await _topImages.GetCount();

            return new TopImageResponse
            {
                Ignored = count.Ignored,
                Total = count.All,
                Images = images
            };
        }

        [HttpPost("ignore")]
        public async Task<IActionResult> UpdateIgnore(IgnoreRequest request)
        {
            await _topImages.Ignore(request.ImageUrl, request.Value);

            return NoContent();
        }
    }

    public class IgnoreRequest
    {
        public bool Value { get; set; }

        public string ImageUrl { get; set; }
    }

    public class TopImageResponse
    {
        public List<TopImage> Images { get; set; }

        public int Total { get; set; }

        public int Ignored { get; set; }
    }
}
