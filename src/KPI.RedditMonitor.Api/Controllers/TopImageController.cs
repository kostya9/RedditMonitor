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

        [HttpPost]
        public async Task<ActionResult<TopImageResponse>> Post(TopImageQueryRequest request)
        {
            var images = await _topImages.GetTop(!request.Ignored ? 12 : 100, request.Ignored, request.Subreddits);
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

    public class TopImageQueryRequest 
    {
        public bool Ignored { get; set; }

        public string[] Subreddits { get; set; }
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
