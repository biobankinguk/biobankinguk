using System.Collections.Generic;

namespace Directory.Entity.Data
{
    public class County
    {
        public int CountyId { get; set; }
        public string Name { get; set; }
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
