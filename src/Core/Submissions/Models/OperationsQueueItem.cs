using System;
using Core.Submissions.Types;

namespace Core.Submissions.Models
{
    public class OperationsQueueItem
    {
        public int SubmissionId { get; set; }
        public Operation Operation { get; set; }

        public Guid BlobId { get; set; }

        public string BlobType { get; set; }

        public int BiobankId { get; set; }
    }
}
