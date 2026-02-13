using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidproblemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]    
    public class ExamDefController : Controller
    {
        private readonly IAuthenticateService _authService;
        private readonly IExamDefinitionService _service;
        private readonly string indicatorPagination = "_first_page_";

        public ExamDefController(IAuthenticateService authService, IExamDefinitionService service)
        {
            _authService = authService;
            _service = service;
        }

        [HttpGet("{category}/{title}")]
        [Authorize(Policy = "Child")]
        public async Task<IActionResult> GetSingleExamDefinition(string category, string title)
        {
            var entity = await _service.GetExamDefinitionAsync(category, title);
            if (entity == null)
            {
                return NotFound(title);
            }
            else
            {
                return Ok(entity);
            }
        }

        [HttpGet("{category}")]
        [Authorize(Policy = "Child")]
        public async Task<IActionResult> QueryExamDefinitions(string category,
            [FromQuery] string? active,
            [FromQuery] string? keyword,
            [FromQuery] int? size,
            [FromQuery] string? pagination)
        {
            bool usePagination = pagination != null || pagination == indicatorPagination;
            bool activeOnly = (active ?? string.Empty).ToLower() == "y";
            pagination = pagination == indicatorPagination ? null : pagination;
            var entity = await _service.QueryExamDefinitionsAsync(category, activeOnly, keyword ?? string.Empty, usePagination, size ?? 25, pagination);
            return Ok(new { data = entity.Item1, pagination = entity.Item2 });
        }

        [HttpPost]
        [Authorize(Policy = "ParentOnly")]
        public async Task<IActionResult> CreateExamDefinition([FromBody] ExamDefinition entity)
        {
            entity = await _service.CreateAsync(entity);
            return Ok(entity);
        }

        [HttpPut("{category}/{title}")]
        [Authorize(Policy = "ParentOnly")]
        public async Task<IActionResult> UpdateExamDefinition(string category, string title, [FromBody] ExamDefinition entity)
        {
            var existing = await _service.GetExamDefinitionAsync(category, title);
            if (existing == null)
            {
                return NotFound(title);
            }
            else
            {
                entity = await _service.UpdateAsync(entity);
                return Ok(entity);
            }
        }

        [HttpDelete("{category}/{title}")]
        [Authorize(Policy = "ParentOnly")]
        public async Task<IActionResult> DeleteExamDefinition(string category, string title)
        {
            var existing = await _service.GetExamDefinitionAsync(category, title);
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

    }
}
