using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KAEGoalWebAPI.Services
{
    public class MissionService : IMissionService
    {
        private readonly ApplicationDbContext _DbContext;

        public MissionService(ApplicationDbContext context)
        {
            _DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<(bool Success, string Message)> ExecutedCodeMissionAsync(int missionId, int userId, string code)
        {
            var mission = await _DbContext.Missions
                .Include(m => m.CodeMission)
                .FirstOrDefaultAsync(m => m.Id == missionId);

            if (mission == null || mission.MissionType != "Code")
            {
                return (false, "Mission not found or is not a Code Mission.");
            }

            if (DateTime.UtcNow < mission.StartDate)
            {
                return (false, "Mission has not started yet.");
            }

            if (DateTime.UtcNow > mission.ExpireDate)
            {
                return (false, "Mission has expired.");
            }

            if (mission.CodeMission == null || mission.CodeMission.Code != code)
            {
                return (false, "Invalid code.");
            }

            var user = await _DbContext.Users
                .Include(u => u.Coins)
                .ThenInclude(c => c.CoinType)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return (false, "User not found.");
            }

            var existingCompletion = await _DbContext.UserMissions
                .AnyAsync(um => um.UserId == userId && um.MissionId == missionId);

            if (existingCompletion)
            {
                return (false, "You have already completed this mission.");
            }

            var kaeacoin = user.Coins.FirstOrDefault(c => c.CoinType.Name == "KAEACoin");
            if (kaeacoin == null)
            {
                return (false, "KAEACoin type is not configured for this user.");
            }

            kaeacoin.Balance += mission.CoinReward;

            var transaction = new CoinTransaction
            {
                UserId = userId,
                CoinTypeId = kaeacoin.CoinTypeId,
                Amount = mission.CoinReward,
                TransactionType = "Mission Reward",
                TransactionDate = DateTime.UtcNow,
                Description = $"Reward for completing mission: {mission.Name}"
            };

            _DbContext.CoinTransactions.Add(transaction);

            var userMission = new UserMission
            {
                UserId = userId,
                MissionId = missionId,
                CompletedAt = DateTime.UtcNow,
            };

            _DbContext.UserMissions.Add(userMission);

            try
            {
                await _DbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log exception (ex) for further investigation
                return (false, "An error occurred while saving mission completion.");
            }

            return (true, $"Mission completed successfully! You earned {mission.CoinReward} KAEACoins.");
        }

        public async Task<(bool Success, string Message)> CreateMissionAsync(MissionCreateModel model)
        {
            try
            {
                if (model.MissionType == "Code" && string.IsNullOrEmpty(model.Code))
                {
                    return (false, "Code mission requires a valid code.");
                }

                if (model.StartDate >= model.ExpireDate)
                {
                    return (false, "Start date must be before the expiration date.");
                }

                var mission = new Mission
                {
                    Name = model.Name,
                    Description = model.Description,
                    MissionType = model.MissionType,
                    StartDate = model.StartDate,
                    ExpireDate = model.ExpireDate,
                    PictureUrl = model.PictureUrl,
                    CoinReward = model.CoinReward,
                    CodeMission = model.MissionType == "Code" ? new CodeMission { Code = model.Code } : null
                };

                _DbContext.Missions.Add(mission);
                await _DbContext.SaveChangesAsync();

                return (true, "Mission created successfully.");
            }
            catch (DbUpdateException ex)
            {
                // Log exception (ex) for further investigation
                return (false, $"An error occurred while creating the mission: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Catch any other exceptions
                return (false, $"An unexpected error occurred: {ex.Message}");
            }
        }

        public async Task<Mission> GetMissionByIdAsync(int missionId)
        {
            return await _DbContext.Missions
                .Include(m => m.CodeMission)
                .FirstOrDefaultAsync(m => m.Id == missionId);
        }

        public async Task<IEnumerable<Mission>> GetAllMissionsAsync()
        {
            return await _DbContext.Missions
                .Include(m => m.CodeMission)
                .ToListAsync();
        }
    }
}
