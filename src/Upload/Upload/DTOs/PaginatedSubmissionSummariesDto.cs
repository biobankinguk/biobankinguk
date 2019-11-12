using System.Collections.Generic;
using Upload.Common.DTO;
using Upload.Common.Models;

namespace Upload.DTOs
{
    public class PaginatedSubmissionSummariesDto : BasePaginatedDto
    {
        public List<SubmissionSummaryDto> Submissions { get; set; } = new List<SubmissionSummaryDto>();
    }
}
