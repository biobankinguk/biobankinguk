using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AnnualStatisticGroup : BaseReferenceData
    {
        public virtual ICollection<AnnualStatistic> AnnualStatistics { get; set; }
    }
}
