using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Admin.Models.Networks;

public class NetworksModel
{
  public ICollection<NetworkModel> Networks { get; set; }
  public string RequestUrl { get; set; } = string.Empty;

}
