using KAEGoalWebAPI.Models;

namespace KAEGoalWebAPI.Services
{
    public interface IWorkplaceService
    {
        Task<Workplace> CreateWorkplaceAsync(string name);
        Task<Workplace> GetWorkplaceByIdAsync(int id);
        Task<IEnumerable<Workplace>> GetAllWorkplacesAsync();
        Task<bool> DeleteWorkplaceAsync(int id);
    }
}
