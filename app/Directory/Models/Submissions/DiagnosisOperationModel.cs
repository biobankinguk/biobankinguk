using Biobanks.Submissions.Api.Models.Submissions;
using Core.Submissions.Models;

namespace Biobanks.Submissions.Api.Models
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
