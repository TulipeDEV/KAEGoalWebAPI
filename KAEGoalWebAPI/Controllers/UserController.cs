using System.Security.Claims;
using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;  // Make sure this contains your User model
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KAEGoalWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _Dbcontext;  // Ensure you have injected the DbContext

        // Constructor to inject the ApplicationDbContext
        public UserController(ApplicationDbContext context)
        {
            _Dbcontext = context;
        }

        [HttpGet("admin-data")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminData()
        {
            return Ok("This is admin-only data.");
        }

        [HttpGet("user-data")]
        [Authorize(Roles = "User, Admin")]
        public IActionResult GetUserData()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User not authenticated");

            // Convert userId to integer if your model's Id is an int
            if (!int.TryParse(userId, out int userIdInt))
            {
                return Unauthorized("Invalid user ID format");
            }

            //var user = _Dbcontext.Users.Find(userIdInt);  // Now using int as the primary key
            //if (user == null)
            //    return Unauthorized("User not found");

            var user = _Dbcontext.Users
                .Include(u => u.Coins)
                .ThenInclude(c => c.CoinType)
                .FirstOrDefault(u => u.Id == userIdInt);

            //if (user == null)
            //    return Unauthorized("User not found");

            //foreach (var coin in user.Coins)
            //{
            //    if (coin.CoinType.Name == "KAEACoin")
            //    {

            //    }   
            //    else if (coin.CoinType.Name == "THANKCoin")
            //    {

            //    }
            //}

            // Assuming you have a `Coin` model that has the `CoinType` and `Balance` properties
            var coinBalances = user.Coins.Select(coin => new
            {
                coinType = coin.CoinType.Name, // Or just coin.CoinType if you want the enum/string
                balance = coin.Balance
            }).ToList();

            return Ok(new { username = user.Username, coins = coinBalances });
        }

        [HttpGet("user-coins")]
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> GetUserCoins()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User not authenticated");

            if (!int.TryParse(userId, out int userIdInt))
            {
                return Unauthorized("Invalid user ID format");
            }

            var user = await _Dbcontext.Users.Include(u => u.Coins) // Including Coins table
                                             .ThenInclude(c => c.CoinType) // To get the coin type info
                                             .FirstOrDefaultAsync(u => u.Id == userIdInt);

            if (user == null)
                return Unauthorized("User not found");

            var userCoins = user.Coins.Select(c => new {
                CoinType = c.CoinType.Name,
                Balance = c.Balance
            }).ToList();

            return Ok(userCoins);
        }
    }
}
