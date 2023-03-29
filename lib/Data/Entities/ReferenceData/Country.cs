using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class Country : BaseReferenceData
    {
        public virtual ICollection<Organisation> Organisations { get; set; }

        public virtual ICollection<County> Counties { get; set; }
    }
}
