using Biobanks.Submissions.Api.Models.Biobank;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Capabilities;

public class CapabilityModel
{
  public int Id { get; set; }
  public string OntologyTerm { get; set; }
  public string Protocols { get; set; }
  public int AnnualDonorExpectation { get; set; }

  public IEnumerable<AssociatedDataSummaryModel> AssociatedData { get; set; }

}
