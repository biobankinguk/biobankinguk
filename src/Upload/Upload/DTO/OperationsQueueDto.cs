using System;
using Upload.Common.Types;

namespace Upload.DTO
{
    public class OperationsQueueDto
    {
        public int SubmissionId { get; set; }
        public Operation Operation { get; set; }

        public Guid BlobId { get; set; }

        public string BlobType { get; set; } = string.Empty;

        public int OrganisationId { get; set; }
    }
}
