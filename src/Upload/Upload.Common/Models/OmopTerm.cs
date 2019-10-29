namespace Upload.Common.Models
{
    /// <summary>
    /// This will be populated by the OMOP cache
    /// </summary>
    public class OmopTerm
    {
        public string Id { get; set; }

        /// <summary>
        /// A friendly description of this term
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Foreign key column.
        /// </summary>
        public int? TagId { get; set; }
    }
}
