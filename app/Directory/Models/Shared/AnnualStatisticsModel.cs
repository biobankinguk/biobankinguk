using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class AnnualStatisticsModel
    {
        public ICollection<AnnualStatisticModel> AnnualStatistics { get; set; }
        public ICollection<AnnualStatisticGroupModel> AnnualStatisticGroups { get; set; }
    }

    public class AnnualStatisticModel
    {
        public int Id { get; set; }

        public int AnnualStatisticGroupId { get; set; }

        public string AnnualStatisticGroupName { get; set; }

        [Required]
        public string Name { get; set; }

        public int SortOrder { get; set; }

        public int UsageCount { get; set; }
    }
}
