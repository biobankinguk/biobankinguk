namespace Upload.DTOs
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Diagnosis and the operation to be applied to it.
    /// </summary>
    public class DiagnosisOperationDto : BaseOperationDto
    {
        /// <summary>
        /// The Diagnosis identity model on which to operate.
        /// </summary>
        public DiagnosisIdDto Diagnosis { get; set; } = null!;
    }
}
