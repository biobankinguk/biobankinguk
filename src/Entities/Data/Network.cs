using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Entities.Data
{
    public class Network
    {
        public int NetworkId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string Url { get; set; } //"Website" in model, changed to "Url" to be consistent with Organisation

        public string Logo { get; set; } //Filepath as per Organisation

        [Required]
        public string Description { get; set; }


        public int SopStatusId { get; set; } //Phil has confirmed a ref table for this, unlike data model
        public SopStatus SopStatus { get; set; }

        public DateTime? LastUpdated { get; set; }

        // Handing over requests to third parties
        public bool ContactHandoverEnabled { get; set; }

        public string HandoverBaseUrl { get; set; }

        public string HandoverOrgIdsUrlParamName { get; set; }

        public bool MultipleHandoverOrdIdsParams { get; set; }

        public bool HandoverNonMembers { get;set; }

        public string HandoverNonMembersUrlParamName { get; set; }

        //1 -> M Navigation properties?
        //Organisations

        public virtual ICollection<OrganisationNetwork> OrganisationNetworks { get; set; }
    }
}
