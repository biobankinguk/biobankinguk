
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class CollectionTypeModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
        public int SortOrder { get; set; }
    }
}
