using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class CollectionPercentagesModel
    {
        public ICollection<CollectionPercentageModel> CollectionPercentages { get; set; }
    }

    public class CollectionPercentageModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public decimal LowerBound { get; set; }

        public decimal UpperBound { get; set; }

        public int SampleSetsCount { get; set; }
    }
}
