using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class CollectionType : BaseReferenceData
    {
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
