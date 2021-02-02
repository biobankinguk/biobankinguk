using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class CollectionPercentage
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }

        public decimal LowerBound { get; set; }

        public decimal UpperBound { get; set; }
    }
}
