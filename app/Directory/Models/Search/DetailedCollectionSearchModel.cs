using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Search;

public class DetailedCollectionSearchModel
{
    public string OntologyTerm { get; set; }
    public string SelectedFacets { get; set; }

    public int BiobankId { get; set; }
    public string BiobankExternalId { get; set; }
    public string BiobankName { get; set; }

    public string LogoName { get; set; }
    public string StorageTemperatureName { get; set; }
    public string MacroscopicAssessmentName { get; set; }
    public string ShowPreservationPercentage { get; set; }

    public IList<DetailedCollectionSearchCollectionModel> Collections { get; set; }
}
