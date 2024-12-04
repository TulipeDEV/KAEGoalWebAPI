using KAEGoalWebAPI.Models;

namespace KAEGoalWebAPI.Services
{
    public interface IDepartmentService
    {
        Task<Department> CreateDepartmentAsync(string name);
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<bool> DeleteDepartmentAsync(int id);
    }
}
