using System.Collections.Generic;

namespace Biobanks.Submissions.Core.Models
{
    public class PaginatedSubmissionSummariesModel : BasePaginatedModel
    {
        public ICollection<SubmissionSummaryModel> Submissions { get; set; }
    }
}
