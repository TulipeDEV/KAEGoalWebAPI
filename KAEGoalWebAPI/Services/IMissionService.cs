using KAEGoalWebAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KAEGoalWebAPI.Services
{
    public interface IMissionService
    {
        Task<(bool Success, string Message)> CreateMissionAsync(MissionCreateModel model);
        Task<(bool Success, string Message)> ExecutedCodeMissionAsync(int missionId, int userId, string code);
        Task<Mission> GetMissionByIdAsync(int missionId);
        Task<IEnumerable<Mission>> GetAllMissionsAsync();
    }
}
