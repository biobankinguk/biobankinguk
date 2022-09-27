using Core.Submissions.Types;

namespace Biobanks.Submissions.Api.Models.Submissions
{
    /// <summary>
    /// Represents the core elements of a viewmodel which provides an entity operation type (e.g. submit, delete).
    /// </summary>
    public abstract class BaseOperationModel
    {
        /// <summary>
        /// Unique identifier of the entity on which to operate.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// The Operation to be applied to the entity.
        /// </summary>
        public Operation Op { get; set; }
    }
}
