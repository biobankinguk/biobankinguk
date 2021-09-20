using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataProcurementTimeframe : BaseReferenceData
    {
        [MaxLength(10)]
        [Required]
        public string DisplayValue { get; set; }
    }
}
