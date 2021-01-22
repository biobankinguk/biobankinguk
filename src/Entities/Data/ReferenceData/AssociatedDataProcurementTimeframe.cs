using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataProcurementTimeframe
    {
        public int AssociatedDataProcurementTimeframeId { get; set; }

        [Required]
        public string Description { get; set; }

        [MaxLength(10)]
        [Required]
        public string DisplayValue { get; set; }

        [Required]
        public int SortOrder { get; set; }

    }
}
