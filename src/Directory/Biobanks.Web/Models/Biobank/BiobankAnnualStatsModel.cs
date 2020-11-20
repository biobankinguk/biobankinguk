using System.Collections.Generic;
using Directory.Entity.Data;

namespace Biobanks.Web.Models.Biobank
{
    public class BiobankAnnualStatsModel
    {
        public IEnumerable<AnnualStatisticGroup> AnnualStatisticGroups { get; set; }
        public ICollection<OrganisationAnnualStatistic> BiobankAnnualStatistics { get; set; }
    }
}
