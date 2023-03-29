using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class County : BaseReferenceData
    {
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
