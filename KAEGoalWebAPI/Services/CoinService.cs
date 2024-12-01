using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KAEGoalWebAPI.Services
{
    public class CoinService
    {
        private readonly ApplicationDbContext _DbContext;

        public CoinService(ApplicationDbContext context)
        {
            _DbContext = context;
        }

        public async Task AddCoinAsync(int userId, int coinTypeId, decimal amount)
        {
            var coin = await _DbContext.Coins
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CoinTypeId == coinTypeId);

            if (coin == null)
            {
                coin = new Coin
                {
                    UserId = userId,
                    CoinTypeId = coinTypeId,
                    Balance = amount
                };
                _DbContext.Coins.Add(coin);
            }
            else
            {
                coin.Balance += amount;
            }

            var transaction = new CoinTransaction
            {
                UserId = userId,
                CoinTypeId = coinTypeId,
                Amount = amount,
                TransactionType = "Earned",
                TransactionDate = DateTime.UtcNow,
                Description = "Coins earned"
            };

            _DbContext.CoinTransactions.Add(transaction);
            await _DbContext.SaveChangesAsync();
        }

        public async Task SpendCoinsAsync(int userId, int coinTypeId, decimal amount)
        {
            var coin = await _DbContext.Coins
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CoinTypeId == coinTypeId);

            if (coin == null || coin.Balance < amount)
            {
                throw new Exception("Insufficient balance or coin type not found");
            }

            coin.Balance -= amount;

            var transaction = new CoinTransaction
            {
                UserId = userId,
                CoinTypeId = coinTypeId,
                Amount = amount,
                TransactionType = "Spent",
                TransactionDate = DateTime.UtcNow,
                Description = "Coins spent"
            };

            _DbContext.CoinTransactions .Add(transaction);
            await _DbContext.SaveChangesAsync();
        }

        public async Task<decimal> GetUserCoinBalanceAsync(int userId, int coinTypeId)
        {
            var coin = await _DbContext.Coins
                .FirstOrDefaultAsync(c => c.UserId == userId && c.CoinTypeId == coinTypeId);
            return coin?.Balance ?? 0;
        }

        public async Task<List<CoinTransaction>> GetUserTransactionHistoryAsync(int userId)
        {
            return await _DbContext.CoinTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }
    }
}
