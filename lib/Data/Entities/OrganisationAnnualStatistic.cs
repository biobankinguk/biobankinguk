using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class OrganisationAnnualStatistic
    {
        [Key, Column(Order = 0)]
        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        [Key, Column(Order = 1)]
        public int AnnualStatisticId { get; set; }
        public virtual AnnualStatistic AnnualStatistic { get; set; }

        [Key, Column(Order = 2)]
        public int Year { get; set; }

        public int? Value { get; set; }
    }
}
