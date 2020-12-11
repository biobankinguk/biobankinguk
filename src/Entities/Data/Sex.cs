using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class Sex
    {
        public int SexId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
