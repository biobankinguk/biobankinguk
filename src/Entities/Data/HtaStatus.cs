using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class HtaStatus
    {
        public int HtaStatusId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
