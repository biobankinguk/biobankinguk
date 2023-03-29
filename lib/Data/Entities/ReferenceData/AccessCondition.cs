using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class AccessCondition : BaseReferenceData
    {
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
