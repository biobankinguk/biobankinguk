using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class CollectionType
    {
        public int CollectionTypeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
