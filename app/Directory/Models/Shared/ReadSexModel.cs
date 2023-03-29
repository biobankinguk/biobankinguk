using System;
namespace Biobanks.Submissions.Api.Models.Shared
{
    public class ReadSexModel : SexModel
    {
        //Sum of all Collections and Capabilities
        public int SexCount { get; set; }
    }
}

