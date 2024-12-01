using KAEGoalWebAPI.Models;
using KAEGoalWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KAEGoalWebAPI.Controllers
{
    public class CoinController : Controller
    {
        private readonly CoinService _coinService;

        public CoinController(CoinService coinService)
        {
            _coinService = coinService;
        }

        [HttpPost("add-coins")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCoins([FromBody] AddCoinsRequest request)
        {
            await _coinService.AddCoinAsync(request.UserId, request.CoinTypeId, request.Amount);
            return Ok("Coins added successfully.");
        }

        [HttpPost("spend-coins")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SpendCoins([FromBody] SpendCoinsRequest request)
        {
            await _coinService.SpendCoinsAsync(request.UserId, request.CoinTypeId, request.Amount);
            return Ok("Coins spent successfully.");
        }

        [HttpGet("User-balance/{userId}")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetUserCoinBalance(int userId, [FromQuery] int coinTypeId)
        {
            var balance = await _coinService.GetUserCoinBalanceAsync(userId, coinTypeId);
            return Ok(new { balance });
        }
    }
}
