using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Shared.ReferenceData
{
    /// <summary>
    /// Sex term.
    /// </summary>
    public class Sex
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
