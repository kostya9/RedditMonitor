using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KPI.RedditMonitor.Application.Similarity;
using KPI.RedditMonitor.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KPI.RedditMonitor.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimilarityController : ControllerBase
    {
        private readonly SimilarityService _service;

        public SimilarityController(SimilarityService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<List<ImagePost>>> GetSimilar()
        {
            var file = HttpContext.Request.Form.Files.First().OpenReadStream();
            return await _service.Find(file);
        }
    }
}
