using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KAEGoalWebAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _DbContext;

        public DepartmentService(ApplicationDbContext context)
        {
            _DbContext = context;
        }

        public async Task<Department> CreateDepartmentAsync(string name)
        {
            var department = new Department { Name = name };
            _DbContext.Departments.Add(department);
            await _DbContext.SaveChangesAsync();
            return department;
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await _DbContext.Departments.FindAsync(id);
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _DbContext.Departments.ToListAsync();
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _DbContext.Departments.FindAsync(id);
            if (department == null) return false;

            _DbContext.Departments.Remove(department);
            await _DbContext.SaveChangesAsync();
            return true;
        }
    }
}
