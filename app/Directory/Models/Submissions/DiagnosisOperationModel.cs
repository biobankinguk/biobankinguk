using Biobanks.Submissions.Models;

namespace Biobanks.Directory.Models.Submissions
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Diagnosis and the operation to be applied to it.
    /// </summary>
    public class DiagnosisOperationModel : BaseOperationModel
    {
        /// <summary>
        /// The Diagnosis identity model on which to operate.
        /// </summary>
        public DiagnosisIdModel Diagnosis { get; set; }
    }
}
