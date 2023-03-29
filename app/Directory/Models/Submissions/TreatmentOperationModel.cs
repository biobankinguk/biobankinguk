using Biobanks.Submissions.Api.Models.Submissions;

namespace Biobanks.Submissions.Api.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Treatment and the operation to be applied to it.
    /// </summary>
    public class TreatmentOperationModel : BaseOperationModel
    {
        /// <summary>
        /// The Treatment identity model on which to operate.
        /// </summary>
        public TreatmentSubmissionModel Treatment { get; set; }
    }
}
