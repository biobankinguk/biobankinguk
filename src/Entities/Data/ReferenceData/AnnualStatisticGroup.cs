using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AnnualStatisticGroup
    {
        public int AnnualStatisticGroupId { get;set; }
        public string Name { get; set; }

        public virtual ICollection<AnnualStatistic> AnnualStatistics { get; set; }
    }
}
