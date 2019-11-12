using System;
using System.Collections.Generic;

namespace Common.Data.Upload
{
    public class Submission
    {
        public int Id { get; set; }

        /// <summary>
        /// Weak Foreign key to Organisation
        /// </summary>
        public int OrganisationId { get; set; }

        public DateTime SubmissionTimestamp { get; set; } = DateTime.UtcNow;

        public int TotalRecords { get; set; }

        public int RecordsProcessed { get; set; }

        public string UploadStatus { get; set; } = string.Empty;

        public DateTime StatusChangeTimestamp { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Error> Errors { get; set; } = null!;
    }
}
