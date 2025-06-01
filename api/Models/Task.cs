using System.Text.Json.Serialization;

namespace api.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } // Task description can be left empty

        [JsonIgnore] // Avoid sending this list to frontend
        public List<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}