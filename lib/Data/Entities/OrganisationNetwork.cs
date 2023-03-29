using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Entities.Data
{
    public class OrganisationNetwork
    {
        public string ExternalID { get; set; }

        [Key, Column(Order = 0)]
        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        [Key, Column(Order = 1)]
        public int NetworkId { get; set; }
        public virtual Network Network { get; set; }

        public DateTime? ApprovedDate { get; set; }
    }
}
