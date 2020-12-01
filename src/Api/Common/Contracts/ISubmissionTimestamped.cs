using System;

namespace Biobanks.Common.Contracts
{
    public interface ISubmissionTimestamped
    {
        DateTimeOffset SubmissionTimestamp { get; set; }
    }
}
