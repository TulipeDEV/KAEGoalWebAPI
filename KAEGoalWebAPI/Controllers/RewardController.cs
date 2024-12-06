using System.Security.Claims;
using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;
using KAEGoalWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KAEGoalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RewardController : ControllerBase
    {
        private readonly RewardService _rewardService;

        public RewardController(RewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllRewards()
        {
            var rewards = await _rewardService.GetAllRewardsAsync();
            var RewardRequestModels = rewards.Select(r => new  RewardRequestModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Cost = r.Cost,
                ImageUrl = r.ImageUrl,
                Quantity = r.Quantity
            }).ToList();

            return Ok(RewardRequestModels);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRewardById(int id)
        {
            var reward = await _rewardService.GetRewardByIdAsync(id);
            if (reward == null)
            {
                return NotFound(new { message = "Reward not found" });
            }

            var rewardRequestModel = new RewardRequestModel
            {
                Id = reward.Id,
                Name = reward.Name,
                Description = reward.Description,
                Cost = reward.Cost,
                ImageUrl = reward.ImageUrl,
                Quantity = reward.Quantity
            };

            return Ok(rewardRequestModel);
        }

        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddReward([FromBody] AddRewardModel model)
        {
            var reward = new Reward
            {
                Name = model.Name,
                Description = model.Description,
                Cost = (int)model.Cost,
                ImageUrl = model.ImageUrl,
                Quantity = model.Quantity
            };

            await _rewardService.AddRewardAsync(reward);
            return Ok("Reward added successfully");
        }

        [HttpDelete("rewardId")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReward(int rewardId)
        {
            var success = await _rewardService.DeleteRewardAsync(rewardId);
            if (!success)
                return NotFound("Reward not found");

            return Ok("Reward deleted successfully");
        }

        [HttpPost("redeem/{rewardId}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> RedeemReward(int rewardId)
        {
            // Validate the rewardId from the route
            if (rewardId <= 0)
            {
                return BadRequest(new { message = "Invalid reward ID" });
            }

            // Get the user ID from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new { message = "Invalid or missing user ID" });
            }

            // Attempt to redeem the reward
            var result = await _rewardService.RedeemRewardAsync(userId, rewardId);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }

            // Return success response
            return Ok(new { message = result.Message });
        }


        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateReward(int rewardid, [FromBody] RewardUpdateModel model)
        {
            var result = await _rewardService.UpdateRewardAsync(rewardid, model);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message});
        }

        [HttpPut("admin/reward/{userRewardId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateRewardStatus(int userRewardId, [FromBody] int newStatusId)
        {
            var success = await _rewardService.UpdateRewardStatusAsync(userRewardId, newStatusId);

            if (!success)
            {
                return BadRequest("Invalid reward or status.");
            }

            return Ok(new { message = "Reward status updated successfully" });
        }

        [HttpGet("user/reward/{userRewardId}/status")]
        [Authorize]
        public async Task<IActionResult> GetUserRewardStatus(int userRewardId)
        {
            var status = await _rewardService.GetUserRewardStatusAsync(userRewardId);

            if (status == null)
            {
                return NotFound("User reward not found.");
            }

            return Ok(new { status }  );
        }
    }
}
