using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required] // Validate that name always has a value
        [StringLength(100, ErrorMessage = "Task name can't be longer than 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters.")]
        public string? Description { get; set; } // Task description can be null

        public bool Done { get; set; } = false; // Boolean to mark task as done (false by default)

        public DateTime DateCreated { get; set; } = DateTime.Now; // Record date of creation

        [JsonIgnore] // Avoid sending this list to frontend
        public List<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}