using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Dtos;
using api.Models;

namespace api.Controllers
{
    /// <summary>
    /// API controller for managing task entities and their associations with tags.
    /// Provides CRUD operations, tag assignments, and "done" status toggling.
    /// </summary>
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        // EF Core database context for tasks and related entities
        private readonly ProgramDbContext _context;

        // Logger for recording events, warnings, and errors
        private readonly ILogger<TasksController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TasksController"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger for this controller.</param>
        public TasksController(ProgramDbContext context, ILogger<TasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Maps a Task entity to a TaskDto.
        /// </summary>
        /// <param name="task">Task entity.</param>
        /// <returns>Mapped TaskDto object.</returns>
        private TaskDto MapToDto(Models.Task task) => new TaskDto
        {
            Id = task.Id,
            Name = task.Name,
            Description = task.Description,
            Done = task.Done,
            DateCreated = task.DateCreated.ToString("dd/MM/yyyy"),
            Tags = task.TaskTags.Select(tt => new TagDto
            {
                Id = tt.Tag.Id,
                Name = tt.Tag.Name,
                Color = tt.Tag.Color
            }).ToList()
        };

        /// <summary>
        /// Creates a new task.
        /// </summary>
        /// <param name="task">Task object from the request body.</param>
        /// <returns>Created task DTO on success; <c>400</c> for invalid data; <c>500</c> for DB errors.</returns>
        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] Models.Task task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tasks.Add(task);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create task."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while creating a task.");
            }

            var dto = MapToDto(task);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, dto);
        }

        /// <summary>
        /// Retrieves all tasks with their associated tags.
        /// </summary>
        /// <returns>List of all task DTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
        {
            var tasks = await _context.Tasks
                .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
                .ToListAsync();

            var tasksDtos = tasks.Select(MapToDto).ToList();
            return Ok(tasksDtos);
        }

        /// <summary>
        /// Retrieves a task by its ID.
        /// Logs a warning if the task is not found.
        /// </summary>
        /// <param name="id">Task ID.</param>
        /// <returns>Task DTO on success, or <c>404</c> if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id); // Logs warning on missing task
                return NotFound();
            }

            var dto = MapToDto(task);
            return Ok(dto);
        }

        /// <summary>
        /// Updates a task by its ID.
        /// Logs a warning if not found, or error if update fails.
        /// </summary>
        /// <param name="id">Task ID from the route.</param>
        /// <param name="updatedTask">Updated Task object from the body.</param>
        /// <returns><c>NoContent</c> on success; <c>400</c> for bad request; <c>404</c> if not found; <c>500</c> for DB errors.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] Models.Task updatedTask)
        {
            if (id != updatedTask.Id)
            {
                return BadRequest("Task ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingTask = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (existingTask == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id); // Logs warning on missing task
                return NotFound();
            }

            existingTask.Name = updatedTask.Name;
            existingTask.Description = updatedTask.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update task."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while updating a task.");
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a task by its ID.
        /// Logs a warning if not found, or error if delete fails.
        /// </summary>
        /// <param name="id">Task ID.</param>
        /// <returns><c>NoContent</c> on success; <c>404</c> if not found; <c>500</c> for DB errors.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id); // Logs warning on missing task
                return NotFound();
            }

            _context.Tasks.Remove(task);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete task."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while deleting a task.");
            }

            return NoContent();
        }

        /// <summary>
        /// Toggles a task's "done" status (done/undone).
        /// Logs a warning if not found, or error if DB fails.
        /// </summary>
        /// <param name="id">Task ID.</param>
        /// <returns>Task ID and new Done status on success; <c>404</c> if not found; <c>500</c> for errors.</returns>
        [HttpPatch("{id}/done")]
        public async Task<IActionResult> ToggleTaskDone(int id)
        {
            var task = await _context.Tasks.FindAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id); // Logs warning on missing task
                return NotFound();
            }

            task.Done = !task.Done;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to toggle task done."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while marking the task done/undone.");
            }

            return Ok(new { task.Id, task.Done });
        }

        /// <summary>
        /// Assigns tags to a task, replacing any existing tags.
        /// </summary>
        /// <param name="id">Task ID.</param>
        /// <param name="tagIds">List of tag IDs to assign.</param>
        /// <returns>Task ID and assigned tag IDs; <c>404</c> if not found; <c>500</c> for errors.</returns>
        [HttpPost("{id}/tags-single")]
        public async Task<IActionResult> AddTagsToTask(int id, [FromBody] List<int> tagIds)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id); // Logs warning on missing task
                return NotFound();
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
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tag to task."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while adding a tag to a task.");
            }

            return Ok(new { TaskId = id, TagIds = tagIds });
        }

        /// <summary>
        /// Assigns tags to a task using a string of tag names. Creates new tags if needed.
        /// </summary>
        /// <param name="id">Task ID.</param>
        /// <param name="dto">TagStringDto containing space-separated tag names.</param>
        /// <returns>Task ID and assigned tag names; <c>404</c> if not found; <c>500</c> for errors.</returns>
        [HttpPost("{id}/tags-multiple")]
        public async Task<IActionResult> AddTagsFromString(int id, [FromBody] TagStringDto dto)
        {
            var tagString = dto.TagString;

            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                _logger.LogWarning("Task with ID {Id} not found.", id); // Logs warning on missing task
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
                    Color = TagColorGenerator.Generate(name)
                }).ToList();

            _context.Tags.AddRange(newTags);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tags to task."); // Logs error if DB fails
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
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add tags to task."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while adding tags to a task.");
            }

            return Ok(new { TaskId = id, Tags = allTags.Select(t => t.Name).ToList() });
        }
    }
}