using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Dtos;

namespace api.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        // Map to context
        private readonly ProgramDbContext _context;
        public TasksController(ProgramDbContext context)
        {
            _context = context;
        }

        // Helper to map Task -> Dto
        private TaskDto MapToDto(Models.Task task) => new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Tags = task.TaskTags.Select(tt => new TagDto
            {
                Id = tt.Tag.Id,
                Name = tt.Tag.Name
            }).ToList()
        };

        // CREATE TASK
        [HttpPost]
        public async Task<ActionResult<Models.Task>> CreateTask([FromBody] Models.Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // return error 400
            }

            _context.Tasks.Add(task); // Add new task to db
            await _context.SaveChangesAsync(); // Save changes to db

            // return 201 created with link to dto
            var dto = MapToDto(task);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, dto);
        }

        // READ ALL TASKS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Task>>> GetAllTasks()
        {
            // Load all tasks & their tags
            var tasks = await _context.Tasks
                .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
                .ToListAsync();

            var tasksDtos = tasks.Select(MapToDto).ToList();
            return Ok(tasksDtos);
        }

        // READ TASK (by id)
        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> GetTask(int id)
        {
            // Find a task by id & include its tags
            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound(); // return error 404
            }

            var dto = MapToDto(task);
            return Ok(dto);
        }

        // UPDATE TASK
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] Models.Task updatedTask)
        {
            if (id != updatedTask.Id)
            {
                return BadRequest("Task ID mismatch"); // handle mismatching ids
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // handle invalid model state
            }

            var existingTask = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTask == null)
                return NotFound(); // return error 404

            existingTask.Name = updatedTask.Name;
            existingTask.Description = updatedTask.Description;

            await _context.SaveChangesAsync();
            return NoContent(); // return 204 when updated
        }

        // DELETE TASK
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound(); // return error 404
            }

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent(); // return 204 when deleted
        }
    }
}
