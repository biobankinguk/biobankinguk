using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Biobank.Models.Settings;

public class AcceptanceModel
{
  public ICollection<NetworkAcceptanceModel> NetworkRequests { get; set; }
}
