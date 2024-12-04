using KAEGoalWebAPI.Models;
using KAEGoalWebAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KAEGoalWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class WorkplaceController : Controller
    { 
        private readonly IWorkplaceService _workplaceService;

        public WorkplaceController(IWorkplaceService workplaceService)
        {
            _workplaceService = workplaceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkplace([FromBody] WorkplaceModel model)
        {
            var workplace = await _workplaceService.CreateWorkplaceAsync(model.Name);
            return CreatedAtAction(nameof(GetWorkplaceById), new { id = workplace.Id }, workplace);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkplaceById(int id)
        {
            var workplace = await _workplaceService.GetWorkplaceByIdAsync(id);
            if (workplace == null) return NotFound();
            return Ok(workplace);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkplaces()
        {
            var workplaces = await _workplaceService.GetAllWorkplacesAsync();
            return Ok(workplaces);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteworkplace(int id)
        {
            var success = await _workplaceService.DeleteWorkplaceAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
