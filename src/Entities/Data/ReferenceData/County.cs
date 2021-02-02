using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class County
    {
        public int Id { get; set; }
        public string Value { get; set; }
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
