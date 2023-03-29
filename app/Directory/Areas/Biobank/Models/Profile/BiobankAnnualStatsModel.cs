using System.Collections.Generic;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Directory.Areas.Biobank.Models.Profile;

public class BiobankAnnualStatsModel
{
    public IEnumerable<AnnualStatisticGroup> AnnualStatisticGroups { get; set; }
    public ICollection<OrganisationAnnualStatistic> BiobankAnnualStatistics { get; set; }
    public int BiobankId;
}
