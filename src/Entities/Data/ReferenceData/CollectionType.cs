using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class CollectionType : ReferenceDataBase
    {
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
