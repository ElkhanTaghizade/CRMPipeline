using CRM_Pipeline.DAL;
using CRM_Pipeline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM_Pipeline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StagesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StagesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetStages()
        {
            var stages = await _context.Stages.Where(x => x.Is_Active == true).ToListAsync();

            if (stages == null || !stages.Any())
            {
                return NotFound("No active stages found.");
            }

            return Ok(stages);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStage(Guid id)
        {
            var stage = await _context.Stages.FindAsync(id);

            if (stage == null)
            {
                return NotFound("Stage not found.");
            }

            if (!stage.Is_Active)
            {
                return BadRequest("Stage is inactive.");
            }

            return Ok(stage);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStage([FromBody] StageCreateDto stageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(stageDto.UserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var stage = new Stages
            {
                Name = stageDto.Name,
                UserId = stageDto.UserId,
                User = user.Name,
                Is_Active = true,
                Total_Revenue = 0
            };

            _context.Stages.Add(stage);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStage), new { id = stage.Id }, stage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStage(Guid id, [FromBody] StageCreateDto stageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stage = await _context.Stages.FindAsync(id);

            if (stage == null)
            {
                return NotFound("Stage not found.");
            }

            var user = await _context.Users.FindAsync(stageDto.UserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            stage.Name = stageDto.Name ?? stage.Name;
            stage.UserId = stageDto.UserId;
            stage.User = user.Name;

            _context.Stages.Update(stage);
            await _context.SaveChangesAsync();

            return Ok(stage);  
        }

        [HttpPost("swap-stages")]
        public async Task<IActionResult> SwapStages([FromBody] SwapStagesDTO stageDto)
        {
            if (stageDto.DraggedStageId == stageDto.TargetStageId)
            {
                return BadRequest("Cannot swap a stage with itself.");
            }

            var draggedStage = await _context.Stages.FindAsync(stageDto.DraggedStageId);
            var targetStage = await _context.Stages.FindAsync(stageDto.TargetStageId);

            if (draggedStage == null || targetStage == null)
            {
                return NotFound("One or both stages not found.");
            }

            var draggedStageLeads = await _context.Leads.Where(l => l.Stage_Id == draggedStage.Id).ToListAsync();
            var targetStageLeads = await _context.Leads.Where(l => l.Stage_Id == targetStage.Id).ToListAsync();

            foreach (var lead in draggedStageLeads)
            {
                lead.Stage_Id = targetStage.Id;
            }

            foreach (var lead in targetStageLeads)
            {
                lead.Stage_Id = draggedStage.Id;
            }

            var tempName = draggedStage.Name;
            draggedStage.Name = targetStage.Name;
            targetStage.Name = tempName;

            var tempRevenue = draggedStage.Total_Revenue;
            draggedStage.Total_Revenue = targetStage.Total_Revenue;
            targetStage.Total_Revenue = tempRevenue;

            var tempIsActive = draggedStage.Is_Active;
            draggedStage.Is_Active = targetStage.Is_Active;
            targetStage.Is_Active = tempIsActive;

            _context.Leads.UpdateRange(draggedStageLeads);
            _context.Leads.UpdateRange(targetStageLeads);
            _context.Stages.Update(draggedStage);
            _context.Stages.Update(targetStage);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Stages and leads swapped successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStage(Guid id)
        {
            var stage = await _context.Stages.FindAsync(id);

            if (stage == null)
            {
                return NotFound("Stage not found.");
            }

            if (!stage.Is_Active)
            {
                return BadRequest("Stage is already inactive.");
            }

            stage.Is_Active = false;
            _context.Stages.Update(stage);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Stage deleted successfully." });
        }
    }
}
