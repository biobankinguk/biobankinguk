using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class County : ReferenceDataBase
    {
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
