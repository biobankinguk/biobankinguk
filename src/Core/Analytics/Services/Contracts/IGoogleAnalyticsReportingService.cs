using Google.Apis.AnalyticsReporting.v4.Data;

using System;

namespace Biobanks.Analytics.Services
{
    /// <summary>
    /// For working with Google AnalyticsReporting API v4
    /// </summary>
    public interface IGoogleAnalyticsReportingService
    {
        /// <summary>
        /// Get a Google AnalyticsReporting DateRange from
        /// parameters describing a reporting period
        /// </summary>
        /// <param name="year"></param>
        /// <param name="endQuarter"></param>
        /// <param name="reportPeriod"></param>
        DateRange PeriodAsDateRange(int year, int endQuarter, int reportPeriod);

        /// <summary>
        /// Get the Start and End Dates from a Google AnalyticsReporting DateRange
        /// as native DateTimeOffsets
        /// </summary>
        /// <param name="dateRange">Source Google AnalyticsReporting DateRange</param>
        (DateTimeOffset startDate, DateTimeOffset endDate) GetDateRangeBounds(DateRange dateRange);
    }
}