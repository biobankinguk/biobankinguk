namespace Upload.DTOs
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Treatment and the operation to be applied to it.
    /// </summary>
    public class TreatmentOperationDto : BaseOperationDto
    {
        /// <summary>
        /// The Treatment identity model on which to operate.
        /// </summary>
        public TreatmentSubmissionDto Treatment { get; set; } = null!;
    }
}
