namespace Upload.Common.DTO
{
    /// <summary>
    /// This will be populated by the OMOP cache
    /// </summary>
    public class OmopTermDto
    {
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// A friendly description of this term
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? TagId { get; set; }
    }
}
