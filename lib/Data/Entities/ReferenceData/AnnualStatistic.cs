using System.Collections.Generic;

namespace Biobanks.Data.Entities.ReferenceData
{
    public class AnnualStatistic : BaseReferenceData
    {
        public int AnnualStatisticGroupId { get; set; }
        public virtual AnnualStatisticGroup AnnualStatisticGroup { get; set; }

        public virtual ICollection<OrganisationAnnualStatistic> OrganisationAnnualStatistics { get; set; }
    }
}
