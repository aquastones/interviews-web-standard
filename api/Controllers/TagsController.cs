using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Data;
using api.Dtos;
using api.Models;

namespace api.Controllers
{
    /// <summary>
    /// API controller for managing tag entities and their associations with tasks.
    /// Provides CRUD operations and tag-to-task queries.
    /// </summary>
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        // EF Core database context for tags and related entities
        private readonly ProgramDbContext _context;

        // Logger for recording events, warnings, and errors
        private readonly ILogger<TagsController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagsController"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="logger">Logger instance for this controller.</param>
        public TagsController(ProgramDbContext context, ILogger<TagsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Maps a Tag entity to its corresponding DTO.
        /// </summary>
        /// <param name="tag">Tag entity to map.</param>
        /// <returns>TagDto representing the tag.</returns>
        private TagDto MapToDto(Tag tag) => new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Color = tag.Color
        };

        /// <summary>
        /// Creates a new tag.
        /// </summary>
        /// <param name="tag">Tag object from the request body.</param>
        /// <returns>Created tag DTO on success; <c>400</c> for invalid data; <c>500</c> for errors.</returns>
        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag([FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Automatically generate a color based on the tag name
            tag.Color = TagColorGenerator.Generate(tag.Name);

            _context.Tags.Add(tag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create a tag."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while creating a tag.");
            }

            var dto = MapToDto(tag);
            return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, dto);
        }

        /// <summary>
        /// Retrieves all tags.
        /// </summary>
        /// <returns>List of tag DTOs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags.Select(MapToDto).ToList());
        }

        /// <summary>
        /// Retrieves a tag by its ID.
        /// Logs a warning if the tag is not found.
        /// </summary>
        /// <param name="id">Tag ID.</param>
        /// <returns>Tag DTO on success, or <c>404</c> if tag not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTagById(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found.", id); // Logs warning on missing tag
                return NotFound();
            }

            return Ok(MapToDto(tag));
        }

        /// <summary>
        /// Retrieves all tasks associated with a specific tag by tag ID.
        /// Logs a warning if the tag is not found.
        /// </summary>
        /// <param name="id">Tag ID.</param>
        /// <returns>List of associated task DTOs on success; <c>404</c> if tag not found.</returns>
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksForTag(int id)
        {
            var tag = await _context.Tags
                .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Task)
                .ThenInclude(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found.", id); // Logs warning on missing tag
                return NotFound();
            }

            var tasks = tag.TaskTags.Select(tt => tt.Task).Distinct().Select(t => new TaskDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Tags = t.TaskTags.Select(tt => new TagDto
                {
                    Id = tt.Tag.Id,
                    Name = tt.Tag.Name
                }).ToList()
            }).ToList();

            return Ok(tasks);
        }

        /// <summary>
        /// Updates a tag by ID.
        /// Logs a warning if tag not found, or error if update fails.
        /// </summary>
        /// <param name="id">Tag ID from the route.</param>
        /// <param name="tag">Updated tag object from the body.</param>
        /// <returns><c>NoContent</c> on success; <c>400</c> for bad request; <c>404</c> if tag not found; <c>500</c> for errors.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != tag.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingTag = await _context.Tags.FindAsync(id);
            if (existingTag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found.", id); // Logs warning on missing tag
                return NotFound();
            }

            existingTag.Name = tag.Name;
            existingTag.Color = TagColorGenerator.Generate(tag.Name);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update tag."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while updating a tag.");
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a tag by ID.
        /// Logs a warning if tag not found, or error if delete fails.
        /// </summary>
        /// <param name="id">Tag ID.</param>
        /// <returns><c>NoContent</c> on success; <c>404</c> if tag not found; <c>500</c> for errors.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                _logger.LogWarning("Tag with ID {Id} not found.", id); // Logs warning on missing tag
                return NotFound();
            }

            _context.Tags.Remove(tag);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete tag."); // Logs error if DB fails
                return StatusCode(500, "An error occurred while deleting a tag.");
            }

            return NoContent();
        }
    }
}