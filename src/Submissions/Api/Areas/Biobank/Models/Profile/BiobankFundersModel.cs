using System.Collections.Generic;
using Biobanks.Submissions.Api.Areas.Admin.Models;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

public class BiobankFundersModel
{
    public int BiobankId { get; set; }
    public ICollection<FunderModel> Funders { get; set; }
}
