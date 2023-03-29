using System;

namespace Biobanks.Entities.Api.Contracts
{
    public interface ISubmissionTimestamped
    {
        DateTimeOffset SubmissionTimestamp { get; set; }
    }
}
