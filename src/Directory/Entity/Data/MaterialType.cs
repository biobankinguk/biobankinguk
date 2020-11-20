using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class MaterialType
    {
        public int MaterialTypeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
