using Biobanks.Submissions.Core.Models;
using System.Collections.Generic;

namespace Core.Submissions.Models
{
    public class PaginatedSubmissionSummariesModel : BasePaginatedModel
    {
        public ICollection<SubmissionSummaryModel> Submissions { get; set; }
    }
}
