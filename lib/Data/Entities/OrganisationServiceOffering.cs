using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class OrganisationServiceOffering
    {
        [Key, Column(Order = 0)]
        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        [Key, Column(Order = 1)]
        public int ServiceOfferingId { get; set; }
        public virtual ServiceOffering ServiceOffering { get; set; }
    }
}
