using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Models;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Http;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

public class BiobankDetailsModel
{
    //non editable stuff that stays the same for edit / empty for create
        public int? BiobankId { get; set; } //nullable to allow for creation of new?
        public string BiobankExternalId { get; set; }
        public int? OrganisationTypeId { get; set; }

        public int? AccessConditionId { get; set; }

        public int? CollectionTypeId { get; set; }

        [Required(ErrorMessage = "Please enter the name of the resource.")]
        [MaxLength(100, ErrorMessage = ModelErrors.MaxLength)]
        [Display(Name = "Name")]
        public string OrganisationName { get; set; }

        [Required(ErrorMessage = "Please enter a description of the resource.")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "URL")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Please enter a contact email address.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Contact email")]
        public string ContactEmail { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Phone]
        [Display(Name = "Contact phone number")]
        public string ContactNumber { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile Logo { get; set; } //how do file uploads work? http://cpratt.co/file-uploads-in-asp-net-mvc-with-view-models/
        public string LogoName { get; set; } //used to display an already stored logo
        public bool RemoveLogo { get; set; }

        //Address
        [Required(ErrorMessage = "Please enter the first line of the resource's address.")]
        [Display(Name = "Address")] //Only provide a display name for the first line which is mandatory
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; } //don't bother labelling the optional lines
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }

        [Required(ErrorMessage = "Please enter the name of a city.")]
        public string City { get; set; }

        [Display(Name = "County")]
        public int? CountyId { get; set; }

        [Display(Name = "County")]
        public string CountyName { get; set; }


        [Required(ErrorMessage = "Please enter a postcode.")]
        [DataType(DataType.PostalCode)]
        public string Postcode { get; set; }

        [Required(ErrorMessage = "Please select a country.")]
        [Display(Name = "Country")]
        public int? CountryId { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }


        //Governing Body
        [Required(ErrorMessage = "Please enter the name of the resource's governing institution.")]
        [MaxLength(100, ErrorMessage = ModelErrors.MaxLength)]
        [Display(Name = "Institution")]
        public string GoverningInstitution { get; set; }
        [MaxLength(100, ErrorMessage = ModelErrors.MaxLength)]
        [Display(Name = "School / Department")]
        public string GoverningDepartment { get; set; }

        [Display(Name = "Ethics Registration")]
        [MaxLength(100, ErrorMessage = ModelErrors.MaxLength)] //consider more accurate validation?
        public string EthicsRegistration { get; set; }

        [Display(Name = "Opt out of sharing data with other directories")]
        public bool SharingOptOut { get; set; }

        [Display(Name = "Other")]
        public string OtherRegistrationReason { get; set; }

        public ICollection<OrganisationServiceModel> ServiceModels { get; set; }

        public ICollection<OrganisationRegistrationReasonModel> RegistrationReasons { get; set; }

        public ICollection<NetworkMemberModel> NetworkModels { get; set; }

        public ICollection<County> Counties { get; set; }
        public ICollection<Country> Countries { get; set; }

        public IEnumerable<AnnualStatisticGroup> AnnualStatisticGroups { get; set; }
        public ICollection<OrganisationAnnualStatistic> BiobankAnnualStatistics { get; set; }
    
}
