using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Network.Models.Settings;

public class NetworkAdminsModel
{
    public int NetworkId { get; set; }

    public List<Directory.Models.Shared.RegisterEntityAdminModel> Admins { get; set; }

}
