using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataTypeGroup : ReferenceDataBase
    {
        public virtual ICollection<AssociatedDataType> AssociatedDataTypes { get; set; }
    }
}
