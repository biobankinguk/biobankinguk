using Biobanks.Submissions.Api.Models.Shared;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.Biobank;

public class BiobankAdminsModel
{
  public int BiobankId { get; set; }

  public ICollection<RegisterEntityAdminModel> Admins { get; set; }

  public string RequestUrl { get; set; } = string.Empty;

}
