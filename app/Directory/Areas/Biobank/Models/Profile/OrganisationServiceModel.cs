namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

public class OrganisationServiceModel
{
    public int ServiceOfferingId { get; set; }

    public string ServiceOfferingName { get; set; }

    public bool Active { get; set; }

    public int SortOrder { get; set; }
}
