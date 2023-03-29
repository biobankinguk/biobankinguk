using System;

namespace Core.Submissions.Types
{
    /// <summary>
    /// Additional parameters for paginating a filtered list of submissions
    /// </summary>
    public class SubmissionPaginationParams : PaginationParams
    {
        /// <summary>
        /// Filter submissions to those since this datetime.
        /// </summary>
        public DateTime? Since { get; set; }

        /// <summary>
        /// Filter submissions to those since the Nth most recent commit
        /// </summary>
        public int N { get; set; }
    }
}
