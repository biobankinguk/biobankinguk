using Biobanks.Submissions.Api.Models.Shared;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Biobank;

public class BiobankAdminsModel
{
    public int BiobankId { get; set; }

    public ICollection<RegisterEntityAdminModel> Admins { get; set; }
  
}
