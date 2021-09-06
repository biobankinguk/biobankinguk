using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class ServiceOffering : ReferenceDataBase
    {
        public virtual ICollection<OrganisationServiceOffering> OrganisationServiceOfferings { get; set; }
    }
}
