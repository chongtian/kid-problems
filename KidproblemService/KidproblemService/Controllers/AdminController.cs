using KidproblemService.Interfaces;
using KidproblemService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KidproblemService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        protected readonly IAuthenticateService _authService;
        protected readonly ICodeService _codeService;
        private readonly string? _tableSuufix;

        public AdminController(IAuthenticateService authService, ICodeService codeService, IOptions<AwsConfiguration> awsConfiguration)
        {
            _authService = authService;
            _codeService = codeService;
            _tableSuufix = awsConfiguration.Value.DynamoDbTableNamePrefix;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { Message= "Service is online.", DynamoDbTableNamePrefix= _tableSuufix });
        }

        [HttpGet("test")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Test()
        {
            string username = _authService.GetCurrentUserInfo().Username!;
            string fullname = await _authService.GetUserNameAsync(username);
            var children = await _authService.GetAllChildrenAsync(_authService.GetCurrentUserInfo().FamilyId!);
            string message = $" User {username} is {fullname}. Children: {string.Join(',', children)}";
            return Ok("Service is online." + message);
        }

        [HttpGet("children")]
        [Authorize(Policy = "ParentOnly")]
        public async Task<IActionResult> GetChildren()
        {
            var children = await _authService.GetAllChildrenAsync(_authService.GetCurrentUserInfo().FamilyId!);
            return Ok(children);
        }

        [HttpGet("codes/{codeName}")]
        [Authorize(Policy = "Child")]
        public async Task<IActionResult> GetCodes(string codeName)
        {
            var entities = await _codeService.GetCodeDetailsAsync(codeName);
            if (entities == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(entities);
            }
        }

        [HttpPut("codes/{codeName}")]
        [Authorize(Policy = "Child")]
        public async Task<IActionResult> UpdateCodes(string codeName, List<CodeDetail> codeDetails)
        {
            if (_codeService.IsValidCodeName(codeName))
            {
                var code = new InfoCentralCode()
                {
                    CodeName = codeName,
                    CodeDetails = codeDetails
                };
                code = await _codeService.UpdateCodeDetailsAsync(code);
                return Ok(code.CodeDetails);
            }
            else
            {
                return BadRequest($"Code Name {codeName} is invalid.");
            }
        }
    }
}