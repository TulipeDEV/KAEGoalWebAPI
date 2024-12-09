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
        public DbSet<Department> Departments { get; set; }
        public DbSet<Workplace> Workplaces { get; set; }
        public DbSet<RewardStatusEntity> RewardStatuses { get; set; }
        public DbSet<UserReward> UserRewards { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<CodeMission> CodeMissions { get; set; }
        public DbSet<UserMission> UserMissions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            RewardStatusSeeder.SeedRewardStatuses(modelBuilder);

            modelBuilder.Entity<UserReward>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRewards)
            .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserReward>()
                .HasOne(ur => ur.Reward)
                .WithMany(r => r.UserRewards)
                .HasForeignKey(ur => ur.RewardId);

            modelBuilder.Entity<UserReward>()
                .HasOne(ur => ur.StatusNavigation)  // Foreign key to RewardStatusEntity
                .WithMany()
                .HasForeignKey(ur => ur.Status);
        }
    }
}
