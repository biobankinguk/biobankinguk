using Biobanks.Submissions.Api.Models.Shared;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.Networks;

public class NetworkModel
{
  public int NetworkId { get; set; }

  public string Name { get; set; }

  public ICollection<RegisterEntityAdminModel> Admins { get; set; }

}
