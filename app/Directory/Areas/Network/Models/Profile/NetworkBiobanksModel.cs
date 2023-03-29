using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Network.Models.Profile;

public class NetworkBiobanksModel
{
  public ICollection<NetworkBiobankModel> Biobanks { get; set; }
}
