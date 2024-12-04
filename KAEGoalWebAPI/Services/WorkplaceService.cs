using KAEGoalWebAPI.Data;
using KAEGoalWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace KAEGoalWebAPI.Services
{
    public class WorkplaceService : IWorkplaceService
    {
        private readonly ApplicationDbContext _DbContext;

        public WorkplaceService(ApplicationDbContext context)
        {
            _DbContext = context;
        }

        public async Task<Workplace> CreateWorkplaceAsync(string name)
        {
            var workplace = new Workplace { Name = name };
            _DbContext.Workplaces.Add(workplace);
            await _DbContext.SaveChangesAsync();
            return workplace;
        }

        public async Task<Workplace> GetWorkplaceByIdAsync(int id)
        {
            return await _DbContext.Workplaces.FindAsync(id);
        }

        public async Task<IEnumerable<Workplace>> GetAllWorkplacesAsync()
        {
            return await _DbContext.Workplaces.ToListAsync();
        }

        public async Task<bool> DeleteWorkplaceAsync(int id)
        {
            var workplace = await _DbContext.Workplaces.FindAsync(id);
            if (workplace == null) return false;

            _DbContext.Workplaces.Remove(workplace);
            await _DbContext.SaveChangesAsync();
            return true;
        }

        //public Task<IEnumerable<Workplace>> GetAllWorkplacesAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
