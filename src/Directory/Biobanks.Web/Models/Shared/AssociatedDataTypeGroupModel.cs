using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
{
    public class AssociatedDataTypeGroupModel
    {
        [Required]
        public int AssociatedDataTypeGroupId { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
