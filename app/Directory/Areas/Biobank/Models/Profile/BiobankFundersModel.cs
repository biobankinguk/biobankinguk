using System.Collections.Generic;
using Biobanks.Directory.Areas.Admin.Models.Funders;

namespace Biobanks.Directory.Areas.Biobank.Models.Profile;

public class BiobankFundersModel
{
    public int BiobankId { get; set; }
    public ICollection<FunderModel> Funders { get; set; }
}
