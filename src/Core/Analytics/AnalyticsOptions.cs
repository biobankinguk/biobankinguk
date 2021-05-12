﻿namespace Biobanks.Analytics
{
    public class AnalyticsOptions
    {
        /// <summary>
        /// If specified, all Analytics Data will be filtered by the provided hostname.
        /// Usually this would be the hostname of the Directory application.
        /// </summary>
        public string FilterHostname { get; set; } = string.Empty;

        /// <summary>
        /// Number of Organisations to include in the ranking for Organisation reports
        /// </summary>
        public int MetricThreshold { get; set; } = 10;
    }
}
