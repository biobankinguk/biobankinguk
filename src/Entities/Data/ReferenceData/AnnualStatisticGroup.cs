using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AnnualStatisticGroup
    {
        public int Id { get;set; }
        public string Value { get; set; }

        public virtual ICollection<AnnualStatistic> AnnualStatistics { get; set; }
    }
}
