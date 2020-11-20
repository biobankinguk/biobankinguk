using System.Collections.Generic;

namespace Directory.Entity.Data
{
    public class AnnualStatisticGroup
    {
        public int AnnualStatisticGroupId { get;set; }
        public string Name { get; set; }

        public virtual ICollection<AnnualStatistic> AnnualStatistics { get; set; }
    }
}
