using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class CollectionAssociatedData
    {
        [Key, Column(Order = 0)]
        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; }

        [Key,Column(Order = 1)]
        public int AssociatedDataTypeId { get; set; }
        public virtual AssociatedDataType AssociatedDataType { get; set; }

        public int? AssociatedDataProcurementTimeframeId { get; set; }
        public virtual AssociatedDataProcurementTimeframe AssociatedDataProcurementTimeframe { get; set; }
    }
}
