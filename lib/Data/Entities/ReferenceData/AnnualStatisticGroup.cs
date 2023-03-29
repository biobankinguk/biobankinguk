using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class AnnualStatisticGroup : BaseReferenceData
    {
        public virtual ICollection<AnnualStatistic> AnnualStatistics { get; set; }
    }
}
