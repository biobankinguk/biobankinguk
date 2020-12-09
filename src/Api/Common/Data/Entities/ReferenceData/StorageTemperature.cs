using System.ComponentModel.DataAnnotations;

namespace Biobanks.Common.Data.Entities.ReferenceData
{
    /// <summary>
    /// Storage Temperature entity.
    /// </summary>
    public class StorageTemperature
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