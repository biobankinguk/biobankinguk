using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Models.Profile
{
    public class BiobankModel
    {
        public string ExternalId { get; set; }

        public string Name { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        public string Logo { get; set; }

        public string Description { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string ContactEmail { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }
        public string CountyName { get; set; }
        public string CountryName { get; set; }
        public string ContactNumber { get; set; }

        public DateTime? LastUpdated { get; set; }

        public ICollection<NetworkMemberModel> NetworkMembers { get; set; }

        public ICollection<string> CapabilityOntologyTerms { get; set; }

        public ICollection<string> CollectionOntologyTerms { get; set; }

        public ICollection<string> Services { get; set; }

        public IEnumerable<AnnualStatisticGroup> AnnualStatisticGroups { get; set; }
        public ICollection<OrganisationAnnualStatistic> BiobankAnnualStatistics { get; set; }
    }
}
