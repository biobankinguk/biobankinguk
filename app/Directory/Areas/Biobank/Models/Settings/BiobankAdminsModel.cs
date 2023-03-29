using System.Collections.Generic;
using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Biobank.Models.Settings;

public class BiobankAdminsModel
{
    public int BiobankId { get; set; }

    public ICollection<RegisterEntityAdminModel> Admins { get; set; }
    
    public string RequestUrl { get; set; } = string.Empty;
    
}
