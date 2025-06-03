namespace api.Dtos
{
    public class TaskDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool Done { get; set; }

        public string DateCreated { get; set; } = string.Empty;

        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}