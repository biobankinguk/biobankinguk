using System.Collections.Generic;

namespace Directory.Entity.Data
{
    public class Funder
    {
        public int FunderId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
