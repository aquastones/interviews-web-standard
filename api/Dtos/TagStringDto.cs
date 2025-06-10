namespace api.Dtos
{
    /// <summary>
    /// DTO for sending a string of tag names, typically space-separated, to assign to a task.
    /// </summary>
    public class TagStringDto
    {
        /// <summary>
        /// Space-separated string of tag names.
        /// </summary>
        public string TagString { get; set; } = string.Empty;
    }
}