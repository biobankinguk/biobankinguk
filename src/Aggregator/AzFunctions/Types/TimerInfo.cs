namespace Biobanks.Aggregator.AzFunctions.Types
{
    public class TimerInfo
    {
        public ScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }
}
