using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class ConsentRestriction : ReferenceDataBase
    {
        public virtual ICollection<Collection> Collections { get; set; }
    }
}
