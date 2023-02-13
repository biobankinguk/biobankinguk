using System.Collections.Generic;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

public class BiobankAnnualStatsModel
{
    public IEnumerable<AnnualStatisticGroup> AnnualStatisticGroups { get; set; }
    public ICollection<OrganisationAnnualStatistic> BiobankAnnualStatistics { get; set; }
    public int BiobankId;
}
