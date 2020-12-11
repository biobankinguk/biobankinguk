using System.Collections.Generic;

namespace Directory.Entity.Data
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
