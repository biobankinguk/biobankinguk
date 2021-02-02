using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Shared.ReferenceData
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


        public int SortOrder { get; set; }
    }
}