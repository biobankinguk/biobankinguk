using Biobanks.Submissions.Api.Models.Submissions;

namespace Biobanks.Submissions.Api.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Represents a Sample and the operation to be applied to it.
    /// </summary>
    public class SampleOperationModel : BaseOperationModel
    {
        /// <summary>
        /// The Sample identity model on which to operate.
        /// </summary>
        public SampleSubmissionModel Sample { get; set; }
    }
}
