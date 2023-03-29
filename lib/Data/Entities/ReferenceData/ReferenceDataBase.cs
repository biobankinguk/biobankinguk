using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities.ReferenceData
{
    public abstract class ReferenceDataBase
    {
        public int Id { get; set; }

        public int SortOrder { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
