using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Dtos;
using api.Models;

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
            Done = task.Done,
            DateCreated = task.DateCreated.ToString("dd/MM/yyyy"), // Date formatting
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

        // MARK AS DONE/UNDONE
        [HttpPatch("{id}/done")]
        public async Task<IActionResult> ToggleTaskDone(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                return NotFound(); // return error 404
            }

            task.Done = !task.Done;
            await _context.SaveChangesAsync();

            return Ok(new {task.Id, task.Done});
        }

        // ASSIGN TAG TO TASK
        [HttpPost("{id}/tags-single")]
        public async Task<IActionResult> AddTagsToTask(int id, [FromBody] List<int> tagIds)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound(); // return error 404
            }

            // Remove existing TaskTag relationships
            task.TaskTags.Clear();

            // Add new TaskTag relationships
            foreach (var tagId in tagIds.Distinct())
            {
                task.TaskTags.Add(new TaskTag { TaskId = id, TagId = tagId });
            }

            await _context.SaveChangesAsync();
            return Ok(new { TaskId = id, TagIds = tagIds }); // for tags-one
        }

        // ASSIGN MULTIPLE TAGS TO TASK AT ONCE
        [HttpPost("{id}/tags-multiple")]
        public async Task<IActionResult> AddTagsFromString(int id, [FromBody] string tagString)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(tagString))
            {
                return BadRequest("Tag string cannot be empty.");
            }

            var tagNames = tagString
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existingTags = await _context.Tags
                .Where(t => tagNames.Contains(t.Name))
                .ToListAsync();

            var existingTagNames = existingTags.Select(t => t.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Create new tags for ones that don't exist
            var newTags = tagNames
                .Where(name => !existingTagNames.Contains(name))
                .Select(name => new Tag { Name = name })
                .ToList();

            _context.Tags.AddRange(newTags);
            await _context.SaveChangesAsync(); // Save new tags to get their IDs

            var allTags = existingTags.Concat(newTags).ToList();

            // Clear old tags and reassign
            task.TaskTags.Clear();
            foreach (var tag in allTags)
            {
                task.TaskTags.Add(new TaskTag { TaskId = id, TagId = tag.Id });
            }

            await _context.SaveChangesAsync();

            return Ok(new { TaskId = id, Tags = allTags.Select(t => t.Name).ToList() });
        }
    }
}