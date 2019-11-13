namespace Upload.DTO
{
    /// <summary>
    /// ViewModel representing an individual error message and the identifying properties of the record it relates to.
    /// </summary>
    public class ErrorDto
    {
        /// <summary>
        /// Unique identifier of the error.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Error message text.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Unique identifiers of the record to which the error relates.
        /// </summary>
        public string RecordIdentifiers { get; set; } = string.Empty;
    }
}