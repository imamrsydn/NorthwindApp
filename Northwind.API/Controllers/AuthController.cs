using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Provider.Interfaces;
using Northwind.ViewModel.Auth;
using System.Security.Claims;

namespace Noerthwind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthProvider _authProvider;

        public AuthController(IAuthProvider authProvider)
        {
            _authProvider = authProvider;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authProvider.LoginAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = await _authProvider.GetProfileAsync(username);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            var result = await _authProvider.ChangePasswordAsync(username, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }
    }
}