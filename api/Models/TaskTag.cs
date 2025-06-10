namespace api.Models
{
    /// <summary>
    /// Association (join) entity connecting tasks and tags (many-to-many relationship).
    /// </summary>
    public class TaskTag
    {
        /// <summary>
        /// The task ID in the relationship.
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// The related task entity.
        /// </summary>
        public Task Task { get; set; } = null!;

        /// <summary>
        /// The tag ID in the relationship.
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// The related tag entity.
        /// </summary>
        public Tag Tag { get; set; } = null!;
    }
}