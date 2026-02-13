using KidproblemService.Helpers;
using KidproblemService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidproblemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamSummaryController : Controller
    {
        private readonly IAuthenticateService _authService;
        private readonly ISummaryService _service;
        private readonly string indicatorPagination = "_first_page_";
        public ExamSummaryController(IAuthenticateService authService, ISummaryService service)
        {
            _authService = authService;
            _service = service;
        }

        [HttpGet("{answerBy}/{category}")]
        [Authorize("Child")]
        public async Task<IActionResult> GetSingleExamSummary(string category, string answerBy)
        {
            var entity = await _service.GetExamSummaryAsync(category, answerBy);
            if (entity == null || entity.FamilyId != _authService.GetCurrentUserInfo().FamilyId)
            {
                return NotFound($"{category}, {answerBy}");
            }
            else
            {
                return Ok(entity.PopulateUserFullName(_authService));
            }
        }

        [HttpGet("{answerBy}")]
        [Authorize("Child")]
        public async Task<IActionResult> QueryExamSummaries(string answerBy,
            [FromQuery] int? size, [FromQuery] string? pagination)
        {
            string familyId = _authService.GetCurrentUserInfo().FamilyId ?? string.Empty;
            bool usePagination = pagination != null || pagination == indicatorPagination;
            pagination = pagination == indicatorPagination ? null : pagination;
            var entity = await _service.QueryExamSummariesAsync(familyId, answerBy, usePagination, size ?? 25, pagination);
            return Ok(new { data = entity.Item1.PopulateUserFullName(_authService), pagination = entity.Item2 });
        }

    }
}
