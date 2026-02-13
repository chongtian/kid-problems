using KidproblemService.Helpers;
using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidproblemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamRunController : Controller
    {
        private readonly IAuthenticateService _authService;
        private readonly IExamRunService _service;
        private readonly IAssignmentService _assignmentService;
        private readonly string indicatorPagination = "_first_page_";

        public ExamRunController(IAuthenticateService authService, IExamRunService service, IAssignmentService assignmentService)
        {
            _authService = authService;
            _service = service;
            _assignmentService = assignmentService;
        }

        [HttpGet("{id}")]
        [Authorize("Child")]
        public async Task<IActionResult> GetSingleExamRun(string id)
        {
            var entity = await _service.GetExamRunAsync(id);
            if (entity == null || entity.FamilyId != _authService.GetCurrentUserInfo().FamilyId)
            {
                return NotFound(id);
            }
            else
            {
                return Ok(entity.PopulateUserFullName(_authService));
            }
        }

        [HttpGet("detail/{id}")]
        [Authorize("Child")]
        public async Task<IActionResult> GetSingleExamRunDetail(string id)
        {
            var entity = await _service.GetExamRunDetailAsync(id);
            if (entity == null)
            {
                return NotFound(id);
            }
            else
            {
                return Ok(entity);
            }
        }

        [HttpGet("query/child")]
        [Authorize("Child")]
        public async Task<IActionResult> QueryExamRuns([FromQuery] string? startTimeValue, [FromQuery] string? endTimeValue,
            [FromQuery] int? size, [FromQuery] string? pagination)
        {
            string answerBy = _authService.GetCurrentUserInfo().Username ?? string.Empty;
            DateTime end = endTimeValue == null ? DateTime.UtcNow : Convert.ToDateTime(endTimeValue);
            DateTime start = startTimeValue == null ? end.AddDays(-14) : Convert.ToDateTime(startTimeValue);
            bool usePagination = pagination != null || pagination == indicatorPagination;
            pagination = pagination == indicatorPagination ? null : pagination;
            var entity = await _service.QueryExamRunsAsync(answerBy, start, end, usePagination, size ?? 25, pagination);
            return Ok(new { data = entity.Item1.PopulateUserFullName(_authService), pagination = entity.Item2 });
        }

        [HttpGet("query/family")]
        [Authorize("ParentOnly")]
        public async Task<IActionResult> QueryExamRunsByFamilyId([FromQuery] string? startTimeValue, [FromQuery] string? endTimeValue,
            [FromQuery] int? size, [FromQuery] string? pagination)
        {
            string familyId = _authService.GetCurrentUserInfo().FamilyId ?? string.Empty;
            DateTime end = endTimeValue == null ? DateTime.UtcNow : Convert.ToDateTime(endTimeValue);
            DateTime start = startTimeValue == null ? end.AddDays(-14) : Convert.ToDateTime(startTimeValue);
            bool usePagination = pagination != null || pagination == indicatorPagination;
            pagination = pagination == indicatorPagination ? null : pagination;
            var entity = await _service.QueryExamRunsByFamilyIdAsync(familyId, start, end, usePagination, size ?? 25, pagination);
            return Ok(new { data = entity.Item1.PopulateUserFullName(_authService), pagination = entity.Item2 });
        }

        [HttpPost("{assignmentId}")]
        [Authorize("Child")]
        public async Task<IActionResult> CreateExamRun(string assignmentId)
        {
            string answerBy = _authService.GetCurrentUserInfo().Username ?? string.Empty;
            var assignment = await _assignmentService.GetAssignmentAsync(assignmentId);
            if (assignment == null || assignment.FamilyId != _authService.GetCurrentUserInfo().FamilyId)
            {
                return NotFound(assignmentId);
            }
            else
            {
                var entity = await _service.CreateExamRunFromAssignmentAsync(assignment, answerBy);
                return Ok(entity.PopulateUserFullName(_authService));
            }
        }

        [HttpDelete("{id}")]
        [Authorize("Child")]
        public async Task<IActionResult> DeleteExamRun(string id)
        {
            var entity = await _service.GetExamRunAsync(id);
            if (entity == null || entity.FamilyId != _authService.GetCurrentUserInfo().FamilyId)
            {
                return NotFound(id);
            }
            else
            {
                entity = await _service.DeleteAsync(entity);
                return Ok(entity);
            }
        }

        [HttpPut("detail/{id}")]
        [Authorize("Child")]
        public async Task<IActionResult> UpdateExamRunDetail(string id, [FromBody] ExamRunDetail entity)
        {
            if (entity.Id != id)
            {
                return NotFound(id);
            }
            else
            {
                entity = await _service.UpdateExamRunDetailAsync(entity);
                return Ok(entity);
            }
        }

        [HttpPut("complete/{id}")]
        [Authorize("Child")]
        public async Task<IActionResult> CompleteExam(string id)
        {
            var existing = await _service.GetExamRunAsync(id);
            if (existing == null || existing.FamilyId != _authService.GetCurrentUserInfo().FamilyId)
            {
                return NotFound(id);
            }
            else
            {
                existing = await _service.CompleteExamAsync(existing);
                return Ok(existing);
            }
        }

    }
}
