using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class AssociatedDataTypeGroup : BaseReferenceData
    {
        public virtual ICollection<AssociatedDataType> AssociatedDataTypes { get; set; }
    }
}
