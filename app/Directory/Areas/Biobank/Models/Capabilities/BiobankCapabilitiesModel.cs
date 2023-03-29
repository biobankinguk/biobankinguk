using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Capabilities;

public class BiobankCapabilitiesModel
{
  public IEnumerable<BiobankCapabilityModel> BiobankCapabilityModels { get; set; }
}
public class BiobankCapabilityModel
{
  public int Id { get; set; }
  public string OntologyTerm { get; set; }
  public string Protocol { get; set; }
}
