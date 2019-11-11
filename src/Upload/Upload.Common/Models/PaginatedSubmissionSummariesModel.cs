using System.Collections.Generic;

namespace Biobanks.Common.Models
{
    public class PaginatedSubmissionSummariesModel : BasePaginatedModel
    {
        public List<SubmissionSummaryModel> Submissions { get; set; } = new List<SubmissionSummaryModel>();
    }
}
