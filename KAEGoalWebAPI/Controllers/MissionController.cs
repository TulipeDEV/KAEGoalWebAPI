using System.Security.Claims;
using KAEGoalWebAPI.Models;
using KAEGoalWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KAEGoalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MissionController : ControllerBase
    {
        private readonly IMissionService _missionService;

        public MissionController(IMissionService missionService)
        {
            _missionService = missionService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateMission([FromBody] MissionCreateModel model)
        {
            var result = await _missionService.CreateMissionAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMissions()
        {
            var missions = await _missionService.GetAllMissionsAsync();
            return Ok(missions);
        }

        [HttpGet("{missionId}")]
        public async Task<IActionResult> GetMissionById(int missionId)
        {
            var mission = await _missionService.GetMissionByIdAsync(missionId);
            if (mission == null)
            {
                return NotFound("Mission not found.");
            }

            return Ok(mission);
        }

        [HttpPost("execute/{missionId}")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ExecuteCodeMission(int missionId, [FromBody] ExecutedCodeMissionModel model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("Invalid token.");

            // Attempt to convert the userId claim to an integer
            if (!int.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid user ID format.");

            var result = await _missionService.ExecutedCodeMissionAsync(missionId, userId, model.Code);  // Use model.Code here
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }
    }
}
