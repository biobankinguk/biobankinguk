using System.Collections.Generic;

namespace Biobanks.Directory.Models.Search;

public class DetailedCapabilitySearchModel
{
    public string OntologyTerm { get; set; }
    public string SelectedFacets { get; set; }

    public int BiobankId { get; set; }
    public string BiobankExternalId { get; set; }
    public string BiobankName { get; set; }

    public string LogoName { get; set; }

    public IEnumerable<DetailedCapabilitySearchCapabilityModel> Capabilities { get; set; }
}
