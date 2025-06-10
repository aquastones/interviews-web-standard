using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// Entity representing a task, which can have multiple tags assigned.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Unique ID for the task (set by the database).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the task (required, max 50 characters).
        /// </summary>
        [Required]
        [StringLength(50, ErrorMessage = "Task name can't be longer than 50 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the task (max 100 characters).
        /// </summary>
        [StringLength(100, ErrorMessage = "Description can't be longer than 100 characters.")]
        public string? Description { get; set; }

        /// <summary>
        /// Indicates whether the task is marked as done. Defaults to false.
        /// </summary>
        public bool Done { get; set; } = false;

        /// <summary>
        /// Date and time the task was created. Automatically set to current time.
        /// </summary>
        public DateTime DateCreated { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property for the many-to-many association with tags.
        /// Marked with JsonIgnore to prevent circular references in API output.
        /// </summary>
        [JsonIgnore]
        public List<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}