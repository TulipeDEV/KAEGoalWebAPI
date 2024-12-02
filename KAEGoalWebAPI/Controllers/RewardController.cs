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
                ImageUrl = r.ImageUrl
            }).ToList();

            return Ok(RewardRequestModels);
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

        [HttpPost("redeem")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> RedeemReward([FromBody] RewardRedeemModel model)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Invalid user ID");
            }

            var result = await _rewardService.RedeemRewardAsync(userId,model.RewardId);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateReward(int rewardid, [FromBody] RewardUpdateModel model)
        {
            var reward = new Reward
            {
                Name = model.Name,
                Description = model.Description,
                Cost = (int)model.Cost,
                ImageUrl = model.ImageUrl
            };

            var result = await _rewardService.RewardUpdateAsync(rewardid, reward);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(new { message = result.Message });
        }
    }
}
