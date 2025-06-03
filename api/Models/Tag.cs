using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Tag name can't be longer than 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(7, MinimumLength = 4, ErrorMessage = "Color code must be 4 to 7 characters (e.g., #fff or #ffffff).")]
        public string Color { get; set; } = "#cccccc"; // Light gray by default

        [JsonIgnore] // Avoid sending this list to frontend
        public List<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}