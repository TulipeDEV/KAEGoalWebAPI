using System.Security.Claims;
using KAEGoalWebAPI.Models;
using KAEGoalWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
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
            var token = await _authService.Register(
                model.Username, 
                model.Password, 
                model.Role,
                model.Firstname,
                model.Lastname,
                model.Displayname,
                model.ProfilePictureUrl
                );

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

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Invalid token.");

            var isUpdated = await _authService.UpdateProfile(int.Parse(userId), model);
            if (!isUpdated)
                return BadRequest("Failed to update profile.");

            return Ok("Profile updated successfully.");
        }

        [HttpGet("User-details")]
        [Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Invalid token.");

            var userDetails = await _authService.GetUserDetails(int.Parse(userId));
            if (userDetails == null)
                return NotFound("User not found.");

            return Ok(userDetails);
        }

        [HttpPut("admin/update-user-details")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserDetails([FromBody] AdminUpdateUserDeltailsModel model)
        {
            if (model.UserId <= 0 || model.DepartmentId <= 0 || model.WorkplaceId <= 0)
                return BadRequest("Invalid input data.");

            try
            {
                var isUpdated = await _authService.UpdateUserDetailsAsync(model);
                if (!isUpdated)
                    return NotFound("User not found.");

                return Ok("User details updated successfully.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Interal server errror: {ex.Message}");
            }
        }

        [HttpGet("user-transactions")]
        [Authorize]
        public async Task<IActionResult> GetUserTransactions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Invalid token.");

            var transactions = await _authService.GetUserTransactions(int.Parse(userId), pageNumber, pageSize);
            return Ok(transactions);
        }
    }
}
