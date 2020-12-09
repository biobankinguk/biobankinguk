using System.Collections.Generic;

namespace Biobanks.Common.Models
{
    public class PaginatedSubmissionSummariesModel : BasePaginatedModel
    {
        public ICollection<SubmissionSummaryModel> Submissions { get; set; }
    }
}
