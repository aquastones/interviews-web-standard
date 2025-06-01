namespace api.Models
{
    public class TaskTag
    {
        public int TaskId { get; set; }
        public Task Task { get; set; } = null!; // = null! to suppress warnings

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!; // = null! to suppress warnings
    }
}