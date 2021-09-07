using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AnnualStatistic : ReferenceDataBase
    {
        public int AnnualStatisticGroupId { get; set; }
        public virtual AnnualStatisticGroup AnnualStatisticGroup { get; set; }

        public virtual ICollection<OrganisationAnnualStatistic> OrganisationAnnualStatistics { get; set; }
    }
}
