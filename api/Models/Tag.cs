using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// Entity representing a tag that can be assigned to one or more tasks.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Unique ID for the tag (set by the database).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the tag (required, max 25 characters).
        /// </summary>
        [Required]
        [StringLength(25, ErrorMessage = "Tag name can't be longer than 25 characters.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Hex color code for the tag. Defaults to light gray (#cccccc). Must be 4–7 characters.
        /// </summary>
        [StringLength(7, MinimumLength = 4, ErrorMessage = "Color code must be 4 to 7 characters (e.g., #fff or #ffffff).")]
        public string Color { get; set; } = "#cccccc";

        /// <summary>
        /// Navigation property for the many-to-many association with tasks.
        /// Marked with JsonIgnore to prevent circular references in API output.
        /// </summary>
        [JsonIgnore]
        public List<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}