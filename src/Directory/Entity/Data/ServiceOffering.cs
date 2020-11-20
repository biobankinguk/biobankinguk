using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class ServiceOffering
    {
        [Key]
        public int ServiceId { get; set; }

        [Required]
        public string Name { get; set; }

        public int SortOrder { get; set; }

        public virtual ICollection<OrganisationServiceOffering> OrganisationServiceOfferings { get; set; }
    }
}
