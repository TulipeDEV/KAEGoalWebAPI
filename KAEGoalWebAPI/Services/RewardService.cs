using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KAEGoalWebAPI.Services
{
    public class RewardService
    {
        private readonly ApplicationDbContext _DbContext;

        public RewardService(ApplicationDbContext context)
        {
            _DbContext = context;
        }

        public async Task<List<Reward>> GetAllRewardsAsync()
        {
            return await _DbContext.Rewards.ToListAsync();
        }

        public async Task<Reward> GetRewardByIdAsync(int id)
        {
            return await _DbContext.Rewards.FindAsync(id);
        }


        public async Task<(bool Success, string Message)> RedeemRewardAsync(int userId, int rewardId)
        {
            var user = await _DbContext.Users
                .Include(u => u.Coins)
                .ThenInclude(c => c.CoinType)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return (false, "User not found");
            }

            var reward = await _DbContext.Rewards.FindAsync(rewardId);
            if (reward == null)
            {
                return (false, "Reward not found");
            }

            if (user.Coins == null || !user.Coins.Any())
            {
                return (false, "User does not have any coins");
            }

            var userCoin = user.Coins.FirstOrDefault(c => c.CoinType.Name == "KAEACoin");
            if (userCoin == null)
            {
                return (false, "User does not have KAEACoin");
            }

            if (userCoin.Balance < reward.Cost)
            {
                return (false, "Insufficient KAEACoin balance to redeem this reward");
            }

            userCoin.Balance -= reward.Cost;

            var transaction = new CoinTransaction
            {
                UserId = userId,
                CoinTypeId = userCoin.CoinTypeId,
                Amount = reward.Cost,
                TransactionType = "Redeem",
                TransactionDate = DateTime.UtcNow,
                Description = $"Redeemed reward: {reward.Name}"
            };

            _DbContext.CoinTransactions.Add(transaction);
            await _DbContext.SaveChangesAsync();

            return (true, "Reward redeemed successfully");
        }

        public async Task AddRewardAsync(Reward reward)
        {
            _DbContext.Rewards.Add(reward);
            await _DbContext.SaveChangesAsync();
        }

        public async Task<bool> DeleteRewardAsync(int rewardId)
        {
            var reward = await _DbContext.Rewards.FindAsync(rewardId);
            if (reward == null)
                return false;

            _DbContext.Rewards.Remove(reward);
            await _DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> RewardUpdateAsync(int rewardId, Reward rewardUpdated)
        {
            var reward = await _DbContext.Rewards.FindAsync(rewardId);
            if (reward == null)
            {
                return (false, "Reward not found");
            }

            reward.Name = rewardUpdated.Name;
            reward.Description = rewardUpdated.Description;
            reward.Cost = rewardUpdated.Cost;
            reward.ImageUrl = rewardUpdated.ImageUrl;

            await _DbContext.SaveChangesAsync();
            return (true, "Reward updated successfully");
        }
    }
}
