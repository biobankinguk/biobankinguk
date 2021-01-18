using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class AgeRange
    {
        public int AgeRangeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
