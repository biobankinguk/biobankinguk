using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class CollectionStatus
    {
        public int CollectionStatusId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
