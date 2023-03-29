using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class AssociatedDataProcurementTimeframe : BaseReferenceData
    {
        [MaxLength(10)]
        [Required]
        public string DisplayValue { get; set; }
    }
}
