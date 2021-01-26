using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class Country
    {
        public int Id { get; set; }
        public string Value { get;set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
        public virtual ICollection<County> Counties { get; set; }
    }
}
