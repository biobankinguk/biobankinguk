using System.Collections.Generic;
using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Admin.Models.Networks;

public class NetworkModel
{
  public int NetworkId { get; set; }

  public string Name { get; set; }

  public ICollection<RegisterEntityAdminModel> Admins { get; set; }

}
