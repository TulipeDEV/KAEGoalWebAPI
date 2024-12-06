using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KAEGoalWebAPI.Data
{
    public class RewardStatusSeeder
    {
        public static void SeedRewardStatuses(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RewardStatusEntity>().HasData(
                new RewardStatusEntity { Id = (int)RewardStatus.RewardRequested, Name = "Reward Requested" },
                new RewardStatusEntity { Id = (int)RewardStatus.AwaitingApproval, Name = "Reward Awaiting Approval" },
                new RewardStatusEntity { Id = (int)RewardStatus.PrizeBeingProcured, Name = "Prize Being Procued" },
                new RewardStatusEntity { Id = (int)RewardStatus.PrizeVerification, Name = "Prize Verification" },
                new RewardStatusEntity { Id = (int)RewardStatus.PrizeReady, Name = "Priz Ready for Pickup" }
            );
        }
    }
}
