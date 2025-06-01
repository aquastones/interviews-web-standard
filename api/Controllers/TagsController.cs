using api.Data;
using api.Dtos;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/tags")]
    public class TagsController : ControllerBase
    {
        // Map to context
        private readonly ProgramDbContext _context;
        public TagsController(ProgramDbContext context)
        {
            _context = context;
        }

        // Helper to map Tag -> Dto
        private TagDto MapToDto(Tag tag) => new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };

        // CREATE TAG
        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag([FromBody] Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            var dto = MapToDto(tag);
            return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, dto);
        }

        // ASSIGN TAG TO TASK
        [HttpPost("/api/tasks/{taskId}/tags")]
        public async Task<IActionResult> AddTagsToTask(int taskId, [FromBody] List<int> tagIds)
        {
            var task = await _context.Tasks
                .Include(t => t.TaskTags)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return NotFound(); // return error 404
            }

            foreach (var tagId in tagIds)
            {
                if (!task.TaskTags.Any(tt => tt.TagId == tagId))
                {
                    task.TaskTags.Add(new TaskTag { TaskId = taskId, TagId = tagId });
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        // READ ALL TAGS
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tags = await _context.Tags.ToListAsync();
            return Ok(tags.Select(MapToDto).ToList());
        }

        // READ TAG BY ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTagById(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            return tag == null ? NotFound() : Ok(MapToDto(tag));
        }

        // GET TASKS FOR A TAG
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
                return NotFound(); // return error 404
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

        // UPDATE TAG
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTag(int id, [FromBody] Tag tag)
        {
            if (id != tag.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingTag = await _context.Tags.FindAsync(id);
            if (existingTag == null)
            {
                return NotFound(); // return error 404
            }

            existingTag.Name = tag.Name;

            await _context.SaveChangesAsync();
            return NoContent(); // return 204
        }


        // DELETE TAG
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);

            if (tag == null)
            {
                return NotFound(); // return error 404
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return NoContent(); // return 204
        }
    }
}