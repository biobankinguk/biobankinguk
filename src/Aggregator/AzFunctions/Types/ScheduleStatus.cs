using System;

namespace Biobanks.Aggregator.AzFunctions.Types
{
    public class ScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
