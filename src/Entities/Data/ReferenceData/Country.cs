using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class Country : ReferenceDataBase
    {
        public virtual ICollection<Organisation> Organisations { get; set; }

        public virtual ICollection<County> Counties { get; set; }
    }
}
