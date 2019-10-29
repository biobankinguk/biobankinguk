namespace Biobanks.SubmissionApi.Models
{
    /// <summary>
    /// ViewModel representing an individual error message and the identifying properties of the record it relates to.
    /// </summary>
    public class ErrorModel
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