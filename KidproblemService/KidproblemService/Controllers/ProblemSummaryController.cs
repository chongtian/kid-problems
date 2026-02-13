using KidproblemService.Helpers;
using KidproblemService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KidproblemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProblemSummaryController : Controller
    {
        private readonly IAuthenticateService _authService;
        private readonly ISummaryService _service;
        private readonly string indicatorPagination = "_first_page_";
        public ProblemSummaryController(IAuthenticateService authService, ISummaryService service)
        {
            _authService = authService;
            _service = service;
        }

        [HttpGet("{answerBy}/{title}")]
        [Authorize("Child")]
        public async Task<IActionResult> GetSingleProblemSummary(string title, string answerBy)
        {
            var entity = await _service.GetProblemSummaryAsync(title, answerBy);
            if (entity == null || entity.FamilyId != _authService.GetCurrentUserInfo().FamilyId)
            {
                return NotFound($"{title}, {answerBy}");
            }
            else
            {
                return Ok(entity.PopulateUserFullName(_authService));
            }
        }

        [HttpGet("category/{category}/query")]
        [Authorize("ParentOnly")]
        public async Task<IActionResult> QueryProblemSummaries(string category, [FromQuery] string? answerBy, [FromQuery] string? correct,
            [FromQuery] string? keyword, [FromQuery] int? size, [FromQuery] string? pagination)
        {
            if (!string.IsNullOrEmpty(answerBy))
            {
                var familyId = _authService.GetCurrentUserInfo().FamilyId!;
                if (! await _authService.VerifyIfChildInFamilyAsync(answerBy, familyId))
                {
                    return Unauthorized(answerBy);
                }
            }
            bool usePagination = pagination != null || pagination == indicatorPagination;
            pagination = pagination == indicatorPagination ? null : pagination;
            var entity = await _service.QueryProblemSummariesAsync(category, answerBy, keyword, correct, usePagination, size ?? 25, pagination);
            return Ok(new { data = entity.Item1.PopulateUserFullName(_authService), pagination = entity.Item2 });
        }

    }
}
