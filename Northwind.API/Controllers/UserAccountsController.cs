using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.UserAccount;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin,Employee")] // SuperAdmin & Employee dapat mengakses
    public class UserAccountsController : ControllerBase
    {
        private readonly IUserAccountProvider _userAccountProvider;

        public UserAccountsController(IUserAccountProvider userAccountProvider)
        {
            _userAccountProvider = userAccountProvider;
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAll([FromQuery] string? username, [FromQuery] string? email, [FromQuery] bool? isActive)
        {
            var res = await _userAccountProvider.GetAllAccountsAsync(username, email, isActive);
            return Ok(res);
        }

        [HttpPost("paged")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetPaged([FromBody] UserAccountFilterDto filter)
        {
            var res = await _userAccountProvider.GetPagedAccountsAsync(filter);
            return Ok(res);
        }

        [HttpGet("{username}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var res = await _userAccountProvider.GetAccountDetailAsync(username);
            return res.Success ? Ok(res) : NotFound(res);
        }

        [HttpPatch("active-status")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ToggleStatus([FromBody] UpdateUserStatusDto req)
        {
            var res = await _userAccountProvider.ToggleUserActiveStatusAsync(req);
            return res.Success ? Ok(res) : BadRequest(res);
        }

        // Endpoint Reset Password khusus SuperAdmin & Employee
        [HttpPost("{username}/reset-password")]
        public async Task<IActionResult> ResetPassword(string username)
        {
            var res = await _userAccountProvider.ResetPasswordAsync(username);
            return res.Success ? Ok(res) : BadRequest(res);
        }
    }
}