using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KPI.RedditMonitor.Api.Similarity;
using KPI.RedditMonitor.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KPI.RedditMonitor.Api.Controllers
{
    [ApiController]
    public class SimilarityController : ControllerBase
    {
        private readonly SimilarityService _service;

        public SimilarityController(SimilarityService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<List<ImagePost>>> GetSimilar([FromForm] IFormFile input)
        {
            return await _service.Find(input.OpenReadStream());
        }
    }
}
