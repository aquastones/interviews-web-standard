namespace api.Dtos
{
    /// <summary>
    /// Data Transfer Object for tags, used to transfer tag data to the frontend.
    /// </summary>
    public class TagDto
    {
        /// <summary>
        /// Unique ID for the tag.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the tag.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Hex color code for the tag. Defaults to light gray (#cccccc) if not set.
        /// </summary>
        public string Color { get; set; } = "#cccccc";
    }
}