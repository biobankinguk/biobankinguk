using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Entities.Data
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
