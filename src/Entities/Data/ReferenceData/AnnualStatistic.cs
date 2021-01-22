using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AnnualStatistic
    {
        public int AnnualStatisticId { get;set; }
        public string Name { get; set; }

        public int AnnualStatisticGroupId { get; set; }

        public virtual AnnualStatisticGroup AnnualStatisticGroup { get; set; }

        public virtual ICollection<OrganisationAnnualStatistic> OrganisationAnnualStatistics { get; set; }
    }
}
