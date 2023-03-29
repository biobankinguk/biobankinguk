using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class AssociatedDataProcurementTimeFrameModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [MaxLength(10)]
        public string DisplayName { get; set; }
        public int SortOrder { get; set; }
    }
}
