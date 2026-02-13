using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidproblemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemController : Controller
    {
        private readonly IAuthenticateService _authService;
        private readonly IProblemService _service;
        private readonly string indicatorPagination = "_first_page_";

        public ProblemController(IAuthenticateService authService, IProblemService service)
        {
            _authService = authService;
            _service = service;
        }

        [HttpGet("{title}")]
        [Authorize(Policy = "Child")]
        public async Task<IActionResult> GetSingleProblem(string title)
        {
            var entity = await _service.GetProblemAsync(title);
            if (entity == null)
            {
                return NotFound(title);
            }
            else
            {
                return Ok(entity);
            }
        }

        [HttpGet]
        [Authorize(Policy = "Child")]
        public async Task<IActionResult> QueryProblems([FromQuery] string? keyword, [FromQuery] string? staging, [FromQuery] int? size, [FromQuery] string? pagination)
        {
            bool isStaging = (staging ?? string.Empty).ToLower().StartsWith("y");
            bool usePagination = pagination != null || pagination == indicatorPagination;
            pagination = pagination == indicatorPagination ? null : pagination;
            var entity = await _service.QueryProblemsAsync(keyword ?? string.Empty, isStaging, usePagination, size ?? 25, pagination);
            return Ok(new { data = entity.Item1, pagination = entity.Item2 });
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateProblem([FromBody] Problem entity)
        {
            entity = await _service.CreateAsync(entity);
            return Ok(entity);
        }

        [HttpPut("{title}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProblem(string title, [FromBody] Problem entity)
        {
            var existing = await _service.GetProblemAsync(title);
            if (existing == null)
            {
                return NotFound(title);
            }
            else
            {
                entity = await _service.UpdateAsync(entity, existing);
                return Ok(entity);
            }
        }

        [HttpPut("bulk/answers")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> BulkUpdateProblemAnswers([FromBody] Problem[] entities)
        {
            foreach (var entity in entities)
            {
                var existing = await _service.GetProblemAsync(entity.ProblemTitle ?? string.Empty);
                if (existing == null)
                {
                    entity.ReturnResult = $"Cannot find Problem {entity.ProblemTitle}.";
                }
                else
                {
                    existing.ProblemAnswer = entity.ProblemAnswer;
                    var r = await _service.UpdateAsync(existing, existing);
                    entity.ReturnResult = r.ReturnResult;
                }
            }
            return Ok(entities);
        }

        [HttpPut("bulk/unstaging")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> BulkUpdateProblemStagingFlags([FromBody] Problem[] entities)
        {
            foreach (var entity in entities)
            {
                var existing = await _service.GetProblemAsync(entity.ProblemTitle ?? string.Empty);
                if (existing == null)
                {
                    entity.ReturnResult = $"Cannot find Problem {entity.ProblemTitle}.";
                }
                else
                {
                    existing.IsStaging = false;
                    var r = await _service.UpdateAsync(existing, existing);
                    entity.ReturnResult = r.ReturnResult;
                }
            }
            return Ok(entities);
        }

        [HttpDelete("{title}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProblem(string title)
        {
            var existing = await _service.GetProblemAsync(title);
            if (existing == null)
            {
                return NotFound(title);
            }
            else
            {
                existing = await _service.DeleteAsync(existing);
                return Ok(existing);
            }
        }

        // POST: api/problem/scrap
        [HttpPost("scrap")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Scrap([FromBody] ScrapDefinition definition)
        {
            var problems = await _service.ScrapAsync(definition);
            return Ok(problems);
        }

        // POST: api/problem/bulk/create
        /// <summary>
        /// This endpoint is called when user has a Json of a list of Problems. 
        /// The client application can convert the Json string and call this endpoints.
        /// </summary>
        /// <param name="problems"></param>
        /// <returns></returns>
        [HttpPost("bulk/create")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> BulkCreate([FromBody] Problem[] problems)
        {
            problems = await _service.ScrapAsync(problems);
            return Ok(problems);
        }

    }
}
