using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Network.Models;

public class NetworkBiobanksModel
{
  public ICollection<NetworkBiobankModel> Biobanks { get; set; }
}
