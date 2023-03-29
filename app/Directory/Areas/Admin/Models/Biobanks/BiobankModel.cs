using System.Collections.Generic;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.Biobanks;

public class BiobankModel
{
  public int BiobankId { get; set; }

  public string BiobankExternalId { get; set; }

  public string Name { get; set; }

  public bool IsSuspended { get; set; }

  public string ContactEmail {get; set; }

  public ICollection<RegisterEntityAdminModel> Admins { get; set; }
  
}
