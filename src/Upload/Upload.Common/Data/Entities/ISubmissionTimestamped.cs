using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Data.Upload
{
    public interface ISubmissionTimestamped
    {
        DateTimeOffset SubmissionTimestamp { get; set; }
    }
}
