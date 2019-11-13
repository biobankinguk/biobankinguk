using System.Collections.Generic;
using Upload.Common.DTO;

namespace Upload.DTO
{
    public class PaginatedSubmissionSummariesDto : BasePaginatedDto
    {
        public List<SubmissionSummaryDto> Submissions { get; set; } = new List<SubmissionSummaryDto>();
    }
}
