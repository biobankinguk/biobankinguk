namespace Biobanks.Analytics
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

        /// <summary>
        /// Number of Event groups that originated from the same location on a particular day
        /// above which will be excluded from plots
        /// </summary>
        public int EventThreshold { get; set; } = 30;

        /// <summary>
        /// Default StartDate for Analytics Records in yyyy-MM-dd (ISO-8601 short) format.
        /// </summary>
        public string StartDate { get; set; } = "2016-01-01";

        /// <summary>
        /// The Id from Google Analytics dashboard of the View from which analytics data should be fetched.
        /// </summary>
        public string GoogleAnalyticsViewId { get; set; }

        /// <summary>
        /// A JSON Service Account Key for Google Analytics Reporting API v4
        /// </summary>
        // https://developers.google.com/analytics/devguides/reporting/core/v4/authorization
        public string GoogleAnalyticsReportingKey { get; set; }
    }
}
