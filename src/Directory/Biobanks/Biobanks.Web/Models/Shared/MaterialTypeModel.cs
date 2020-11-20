using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
{
    public class MaterialTypeModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int SortOrder { get; set; }

    }
}