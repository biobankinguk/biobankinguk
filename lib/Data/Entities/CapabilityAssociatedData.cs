using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class CapabilityAssociatedData
    {
        [Key, Column(Order = 0)]
        public int DiagnosisCapabilityId { get; set; }
        public virtual DiagnosisCapability DiagnosisCapability { get; set; }

        [Key, Column(Order = 1)]
        public int AssociatedDataTypeId { get; set; }
        public virtual AssociatedDataType AssociatedDataType { get; set; }

        public int? AssociatedDataProcurementTimeframeId { get; set; }
        public virtual AssociatedDataProcurementTimeframe AssociatedDataProcurementTimeframe { get; set; }
    }
}
