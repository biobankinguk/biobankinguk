using Upload.Common.Types;

namespace Upload.DTOs
{
    /// <summary>
    /// Represents the core elements of a DTO which provides an entity operation type (e.g. submit, delete).
    /// </summary>
    public abstract class BaseOperationDto
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
