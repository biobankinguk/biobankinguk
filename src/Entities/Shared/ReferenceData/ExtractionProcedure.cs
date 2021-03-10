using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Shared.ReferenceData
{
    public class ExtractionProcedure
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public int MaterialTypeId { get; set; }

        public virtual MaterialType MaterialType { get; set; }
    }
}
