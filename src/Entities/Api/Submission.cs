using System;
using System.Collections.Generic;
using Biobanks.Entities.Api.ReferenceData;

namespace Biobanks.Entities.Api
{
    public class Submission
    {
        public int Id { get; set; }

        public int BiobankId { get; set; } // TODO This can become a strong Relationship in the unified Data Model

        public DateTime SubmissionTimestamp { get; set; } = DateTime.UtcNow;

        public int TotalRecords { get; set; }

        public int RecordsProcessed { get; set; }

        public int StatusId { get; set; }
        public Status Status { get; set; }

        public DateTime StatusChangeTimestamp { get; set; } = DateTime.UtcNow;

        public ICollection<Error> Errors { get; set; }
    }
}
