using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class MaterialType
    {
        public int MaterialTypeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
