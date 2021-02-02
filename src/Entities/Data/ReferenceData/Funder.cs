using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class Funder
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
