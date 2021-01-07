using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class PreservationType
    {
        public int PreservationTypeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
