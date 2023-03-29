using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Admin.Models.Requests;

public class HistoricalModel
{
  public ICollection<HistoricalRequestModel> HistoricalRequests { get; set; }
}
