using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class ServiceOffering : BaseReferenceData
    {
        public virtual ICollection<OrganisationServiceOffering> OrganisationServiceOfferings { get; set; }
    }
}
