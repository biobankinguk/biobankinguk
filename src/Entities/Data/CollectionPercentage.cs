using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class CollectionPercentage
    {
        public int CollectionPercentageId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public decimal LowerBound { get; set; }

        public decimal UpperBound { get; set; }
    }
}
