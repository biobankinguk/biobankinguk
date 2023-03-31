using System;

namespace Biobanks.Data.Entities.Api.Contracts
{
    public interface ISubmissionTimestamped
    {
        DateTimeOffset SubmissionTimestamp { get; set; }
    }
}
