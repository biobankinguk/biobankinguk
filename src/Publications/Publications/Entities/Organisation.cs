using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Directory.Data.Entities
{
    public class Organisation
    {
        public int OrganisationId { get; set; } //PK; no required needed, this is a string in the model

        public bool IsSuspended { get; set; }

        [Required]
        public string OrganisationExternalId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ContactEmail { get; set; }

        public string ContactNumber { get; set; }

        public string Logo { get; set; } //Filename, not storing the image in the db
        public string Url { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string PostCode { get; set; }
        public string City { get; set; }

        public int? CountyId { get; set; }
        public int? CountryId { get; set; }

        public int OrganisationTypeId { get; set; }

        public string GoverningInstitution { get; set; }
        public string GoverningDepartment { get; set; }

        public bool SharingOptOut { get; set; }

        public string EthicsRegistration { get; set; }

        public string HtaLicence { get; set; }

        public DateTime? LastUpdated { get; set; }

        public Guid? AnonymousIdentifier { get; set; } = new Guid();

        public string OtherRegistrationReason { get; set; }
        public bool? IncludePublications { get; set; }
    }
}
