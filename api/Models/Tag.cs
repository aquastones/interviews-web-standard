using System.Text.Json.Serialization;

namespace api.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [JsonIgnore] // Avoid sending this list to frontend
        public List<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    }
}