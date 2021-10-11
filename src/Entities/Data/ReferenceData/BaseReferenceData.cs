using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public abstract class BaseReferenceData
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
