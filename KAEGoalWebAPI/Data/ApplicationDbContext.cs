using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KAEGoalWebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<CoinTransaction> CoinTransactions { get; set; }
        public DbSet<CoinType> CoinTypes { get; set; }
        public DbSet<Reward> Rewards { get; set; }
    }
}
