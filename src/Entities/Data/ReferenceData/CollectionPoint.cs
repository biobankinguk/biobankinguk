using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class CollectionPoint
    {
        public int CollectionPointId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
