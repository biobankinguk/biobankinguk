using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataProcurementTimeframe
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        [MaxLength(10)]
        [Required]
        public string DisplayValue { get; set; }

        [Required]
        public int SortOrder { get; set; }

    }
}
