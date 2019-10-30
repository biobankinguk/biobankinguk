using System;
using System.Collections.Generic;

namespace Common.Data.Upload
{
    public class Submission
    {
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to Biobank
        /// </summary>
        public int BiobankId { get; set; }

        public DateTime SubmissionTimestamp { get; set; } = DateTime.UtcNow;

        public int TotalRecords { get; set; }

        public int RecordsProcessed { get; set; }

        /// <summary>
        /// Foreign key to reference data. 
        /// TODO figure out what to do with this status type- not on the list
        /// </summary>
        public int StatusId { get; set; }
        public virtual UploadStatus UploadStatus{get; set;}

        public DateTime StatusChangeTimestamp { get; set; } = DateTime.UtcNow;

        public ICollection<Error> Errors { get; set; }
    }
}
