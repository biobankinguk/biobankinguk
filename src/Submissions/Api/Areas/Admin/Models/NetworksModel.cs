using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models;

public class NetworksModel
{
  public ICollection<NetworkModel> Networks { get; set; }
  public string RequestUrl { get; set; } = string.Empty;

}