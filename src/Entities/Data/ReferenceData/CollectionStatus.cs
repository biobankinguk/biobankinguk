using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class CollectionStatus
    {
        public int CollectionStatusId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
