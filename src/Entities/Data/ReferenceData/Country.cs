using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class Country
    {
        public int CountryId { get; set; }
        public string Name { get;set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
        public virtual ICollection<County> Counties { get; set; }
    }
}
