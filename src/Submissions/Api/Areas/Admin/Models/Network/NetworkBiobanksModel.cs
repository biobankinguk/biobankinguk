using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.Network;

public class NetworkBiobanksModel
{
  public ICollection<NetworkBiobankModel> Biobanks { get; set; }

}
