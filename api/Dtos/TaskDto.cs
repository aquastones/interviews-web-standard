namespace api.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}