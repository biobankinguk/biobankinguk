using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Settings;

public class AcceptanceModel
{
  public ICollection<NetworkAcceptanceModel> NetworkRequests { get; set; }
}
