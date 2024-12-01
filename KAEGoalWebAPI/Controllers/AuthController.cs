using KAEGoalWebAPI.Models;
using KAEGoalWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace KAEGoalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var token = await _authService.Register(model.Username, model.Password, model.Role);
            if (token == null) return BadRequest("User already exists.");
            return Ok(new {Token = token});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var token = await _authService.Login(model.Username, model.Password);
            if (token == null) return Unauthorized("Invalid username or password.");
            return Ok(new { Token = token });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var newJwtToken = _authService.RefreshToken(refreshToken);
            if (newJwtToken == null) return Unauthorized("Invalid or expired refresh token.");
            return Ok(new { Token = newJwtToken});
        }
    }
}
