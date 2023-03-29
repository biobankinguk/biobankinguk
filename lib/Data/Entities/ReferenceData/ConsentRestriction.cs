using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class ConsentRestriction : BaseReferenceData
    {
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
