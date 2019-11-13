namespace Upload.DTO
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Sample and the operation to be applied to it.
    /// </summary>
    public class SampleOperationDto : BaseOperationDto
    {
        /// <summary>
        /// The Sample identity model on which to operate.
        /// </summary>
        public SampleSubmissionDto Sample { get; set; } = null!;
    }
}
