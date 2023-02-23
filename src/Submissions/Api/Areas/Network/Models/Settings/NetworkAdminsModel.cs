using System.Collections.Generic;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Network.Models.Settings;

public class NetworkAdminsModel
{
    public int NetworkId { get; set; }

    public List<Api.Models.Shared.RegisterEntityAdminModel> Admins { get; set; }

}
