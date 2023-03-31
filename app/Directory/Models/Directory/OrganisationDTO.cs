namespace Biobanks.Directory.Models.Directory
{
    public class OrganisationDTO
    { 
    //non editable stuff that stays the same for edit / empty for create
    public int OrganisationId { get; set; } //nullable to allow for creation of new?
    public string OrganisationExternalId { get; set; }
    public int OrganisationTypeId { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Url { get; set; }

    public string ContactEmail { get; set; }

    public string ContactNumber { get; set; }

    public string Logo { get; set; } //used to display an already stored logo

    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; } //don't bother labelling the optional lines
    public string AddressLine3 { get; set; }
    public string AddressLine4 { get; set; }

    public string City { get; set; }

    public string PostCode { get; set; }

    public int? CountyId { get; set; }
    public int? CountryId { get; set; }

    public bool SharingOptOut { get; set; }

    public bool IsSuspended { get; set; }

    //Governing Body
    public string GoverningInstitution { get; set; }
    public string GoverningDepartment { get; set; }

    public string EthicsRegistration { get; set; }

    // registration reasons
    public string OtherRegistrationReason { get; set; }
    public bool ExcludePublications { get; set; }

    public int? AccessConditionId { get; set; }

    public int? CollectionTypeId { get; set; }
}
}
