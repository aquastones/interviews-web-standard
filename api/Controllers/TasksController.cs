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
        private readonly ILogger<TasksController> _logger;

        public TasksController(ProgramDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
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
                Name = tt.Tag.Name,
                Color = tt.Tag.Color
            }).ToList()
        };

        // Color selection for tags
        private static readonly string[] TagColors = new[]
        {
            "#e0ca3c", // yellow
            "#3e2f5b", // violet
            "#e01a4f", // pink
            "#00c2d1", // blue
            "#fc814a", // coral
            "#d6ff79", // green
            "#b0ff92", // light green
            "#368f8b", // cyan
            "#984447", // red
            "#5f00ba", // purple
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
            try
            {
                await _context.SaveChangesAsync(); // Save changes to db
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create task.");
                return StatusCode(500, "An error occurred while creating a task.");
            }

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
            return Ok(tasksDtos); // return 200
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
                _logger.LogWarning("Task with ID {Id} not found.", id);
                return NotFound(); // return error 404
            }

            var dto = MapToDto(task);
            return Ok(dto); // return 200
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
            {
                _logger.LogWarning("Task with ID {Id} not found.", id);
                return NotFound(); // return error 404
            }

            existingTask.Name = updatedTask.Name;
            existingTask.Description = updatedTask.Description;

            try
            {
                await _context.SaveChangesAsync(); // Save changes to db
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update task.");
                return StatusCode(500, "An error occurred while updating a task.");
            }

            return NoContent(); // return 204 when updated
        }

        // DELETE TASK
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id);
                return NotFound(); // return error 404
            }

            _context.Tasks.Remove(task);
            try
            {
                await _context.SaveChangesAsync(); // Save changes to db
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete task.");
                return StatusCode(500, "An error occurred while deleting a task.");
            }

            return NoContent(); // return 204 when deleted
        }

        // MARK AS DONE/UNDONE
        [HttpPatch("{id}/done")]
        public async Task<IActionResult> ToggleTaskDone(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id);
                return NotFound(); // return error 404
            }

            task.Done = !task.Done;
            try
            {
                await _context.SaveChangesAsync(); // Save changes to db
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to toggle task done.");
                return StatusCode(500, "An error occurred while marking the task done/undone.");
            }

            return Ok(new { task.Id, task.Done }); // return 200
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
                _logger.LogWarning("Task with ID {Id} not found.", id);
                return NotFound(); // return error 404
            }

            // Remove existing TaskTag relationships
            task.TaskTags.Clear();

            // Add new TaskTag relationships
            foreach (var tagId in tagIds.Distinct())
            {
                task.TaskTags.Add(new TaskTag { TaskId = id, TagId = tagId });
            }

            try
            {
                await _context.SaveChangesAsync(); // Save changes to db
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tag task.");
                return StatusCode(500, "An error occurred while adding a tag to a task.");
            }

            return Ok(new { TaskId = id, TagIds = tagIds }); // return 200
        }

        // ASSIGN MULTIPLE TAGS TO TASK AT ONCE
        [HttpPost("{id}/tags-multiple")]
        public async Task<IActionResult> AddTagsFromString(int id, [FromBody] TagStringDto dto)
        {
            var tagString = dto.TagString;

            var random = new Random();

            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id);
                return NotFound();
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
                .Select(name => new Tag
                {
                    Name = name,
                    Color = TagColors[random.Next(TagColors.Length)]
                }).ToList();

            _context.Tags.AddRange(newTags);
            try
            {
                await _context.SaveChangesAsync(); // Save new tags (to get ids)
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tags to task.");
                return StatusCode(500, "An error occurred while adding tags to a task.");
            }

            var allTags = existingTags.Concat(newTags).ToList();

            // Clear old tags and reassign
            task.TaskTags.Clear();
            foreach (var tag in allTags)
            {
                task.TaskTags.Add(new TaskTag { TaskId = id, TagId = tag.Id });
            }

            try
            {
                await _context.SaveChangesAsync(); // Save changes to db
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tags to task.");
                return StatusCode(500, "An error occurred while adding tags to a task.");
            }

            return Ok(new { TaskId = id, Tags = allTags.Select(t => t.Name).ToList() }); // return 200
        }
    }
}