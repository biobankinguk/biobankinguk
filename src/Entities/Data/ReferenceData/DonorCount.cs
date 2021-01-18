using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class DonorCount
    {
        public int DonorCountId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int? LowerBound { get; set; }

        public int? UpperBound { get; set; }
    }
}
