using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class HtaStatus
    {
        public int HtaStatusId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
