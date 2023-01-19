using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.Requests;

public class HistoricalModel
{
  public ICollection<HistoricalRequestModel> HistoricalRequests { get; set; }
}
