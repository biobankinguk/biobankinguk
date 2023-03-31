using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class AssociatedDataTypeGroupModel
    {

        [Required]
        public int AssociatedDataTypeGroupId { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
