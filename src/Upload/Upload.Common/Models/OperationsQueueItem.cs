﻿using System;
using Upload.Common.Types;

namespace Biobanks.Common.Models
{
    public class OperationsQueueItem
    {
        public int SubmissionId { get; set; }
        public Operation Operation { get; set; }

        public Guid BlobId { get; set; }

        public string BlobType { get; set; } = string.Empty;

        public int BiobankId { get; set; }
    }
}
