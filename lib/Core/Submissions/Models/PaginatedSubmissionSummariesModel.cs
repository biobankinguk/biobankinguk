using System.Collections.Generic;

namespace Biobanks.Submissions.Models
{
    public class PaginatedSubmissionSummariesModel : BasePaginatedModel
    {
        public ICollection<SubmissionSummaryModel> Submissions { get; set; }
    }
}
