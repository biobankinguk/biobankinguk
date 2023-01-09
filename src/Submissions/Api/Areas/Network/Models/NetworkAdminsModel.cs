using System.Collections.Generic;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Network.Models;

public class NetworkAdminsModel
{
    public int NetworkId { get; set; }

    public ICollection<RegisterEntityAdminModel> Admins { get; set; }

}
