using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class CollectionType : BaseReferenceData
    {
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
