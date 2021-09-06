using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataProcurementTimeframe : ReferenceDataBase
    {
        [MaxLength(10)]
        [Required]
        public string DisplayValue { get; set; }
    }
}
