using System;

namespace Entities.Api.Contracts
{
    public interface ISubmissionTimestamped
    {
        DateTimeOffset SubmissionTimestamp { get; set; }
    }
}
