using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class ServiceOffering : BaseReferenceData
    {
        public virtual ICollection<OrganisationServiceOffering> OrganisationServiceOfferings { get; set; }
    }
}
