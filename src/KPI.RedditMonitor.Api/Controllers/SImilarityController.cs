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
    [Route("api/[controller]")]
    public class SimilarityController : ControllerBase
    {
        private readonly SimilarityService _service;

        public SimilarityController(SimilarityService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<List<ImagePost>>> GetSimilar(IFormFile file)
        {
            return await _service.Find(file.OpenReadStream());
        }
    }
}
