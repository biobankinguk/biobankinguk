using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class SopStatus
    {
        public int SopStatusId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

    }
}
