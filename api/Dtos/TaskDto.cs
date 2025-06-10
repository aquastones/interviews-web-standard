namespace api.Dtos
{
    /// <summary>
    /// Data Transfer Object for tasks, used to transfer task data to the frontend.
    /// </summary>
    public class TaskDto
    {
        /// <summary>
        /// Unique ID for the task.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the task.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Optional description of the task.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Indicates if the task is marked as done.
        /// </summary>
        public bool Done { get; set; }

        /// <summary>
        /// Date the task was created (formatted as dd/MM/yyyy).
        /// </summary>
        public string DateCreated { get; set; } = string.Empty;

        /// <summary>
        /// List of tags assigned to the task.
        /// </summary>
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}