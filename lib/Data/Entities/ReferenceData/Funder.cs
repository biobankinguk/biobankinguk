using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class Funder : BaseReferenceData
    {
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
