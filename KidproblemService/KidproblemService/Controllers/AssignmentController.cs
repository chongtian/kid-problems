using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidproblemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssignmentController : Controller
    {
        private readonly IAuthenticateService _authService;
        private readonly IAssignmentService _service;
        private readonly string indicatorPagination = "_first_page_";

        public AssignmentController(IAuthenticateService authService, IAssignmentService service)
        {
            _authService = authService;
            _service = service;
        }

        [HttpGet("{id}")]
        [Authorize("Child")]
        public async Task<IActionResult> GetSingleAssignment(string id)
        {
            var entity = await _service.GetAssignmentAsync(id);
            if (entity == null)
            {
                return NotFound(id);
            }
            else
            {
                return Ok(entity);
            }
        }

        [HttpGet]
        [Authorize("Child")]
        public async Task<IActionResult> QueryAssignments([FromQuery] string? startTimeValue, [FromQuery] string? endTimeValue,
            [FromQuery] int? size,
            [FromQuery] string? pagination)
        {
            var currentUser = _authService.GetCurrentUserInfo();
            string familyId = currentUser.FamilyId ?? string.Empty;

            // child user has access 1
            string? childId = currentUser.Access == 1 ? currentUser.Username : null;

            DateTime end = endTimeValue == null ? DateTime.UtcNow : Convert.ToDateTime(endTimeValue);
            DateTime start = startTimeValue == null ? end.AddDays(-14) : Convert.ToDateTime(startTimeValue);
            bool usePagination = pagination != null || pagination == indicatorPagination;
            pagination = pagination == indicatorPagination ? null : pagination;
            var entity = await _service.QueryAssignmentsAsync(familyId, childId, start, end, usePagination, size ?? 25, pagination);
            return Ok(new { data = entity.Item1, pagination = entity.Item2 });
        }

        [HttpPost]
        [Authorize("ParentOnly")]
        public async Task<IActionResult> CreateAssignmentFromDefinition([FromBody] ExamDefinition definition)
        {
            string familyId = _authService.GetCurrentUserInfo().FamilyId ?? string.Empty;

            var entity = await _service.CreateAssignmentFromDefinitionAsync(definition, familyId);
            return Ok(entity);
        }

        [HttpPut("{id}")]
        [Authorize("ParentOnly")]
        public async Task<IActionResult> UpdateAssignment(string id, [FromBody] Assignment entity)
        {
            string familyId = _authService.GetCurrentUserInfo().FamilyId ?? string.Empty;
            var existing = await _service.GetAssignmentAsync(id);
            if (existing == null)
            {
                return NotFound(id);
            }
            else if (existing.FamilyId != familyId)
            {
                return Unauthorized(id);
            }
            else
            {
                entity = await _service.UpdateAsync(entity, existing);
                return Ok(entity);
            }
        }

        [HttpDelete("{id}")]
        [Authorize("ParentOnly")]
        public async Task<IActionResult> DeleteAssignment(string id)
        {
            string familyId = _authService.GetCurrentUserInfo().FamilyId ?? string.Empty;
            var existing = await _service.GetAssignmentAsync(id);
            if (existing == null)
            {
                return NotFound(id);
            }
            else if (existing.FamilyId != familyId)
            {
                return Unauthorized(id);
            }
            else
            {
                existing = await _service.DeleteAsync(existing);
                return Ok(existing);
            }
        }

    }
}
