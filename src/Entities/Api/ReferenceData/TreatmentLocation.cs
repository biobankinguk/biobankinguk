using System.ComponentModel.DataAnnotations;

namespace Entities.Api.ReferenceData
{
    /// <summary>
    /// Treatment Location entity.
    /// </summary>
    public class TreatmentLocation
    {
        /// <summary>
        /// Internal id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Value for the term (i.e. friendly name)
        /// </summary>
        [Required]
        public string Value { get; set; }
    }
}