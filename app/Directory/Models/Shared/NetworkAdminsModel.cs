using System.Collections.Generic;

namespace Biobanks.Directory.Models.Shared;

public class NetworkAdminsModel
{
  public int NetworkId { get; set; }

  public ICollection<RegisterEntityAdminModel> Admins { get; set; }
}
