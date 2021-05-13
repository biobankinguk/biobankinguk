using Google.Apis.AnalyticsReporting.v4.Data;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Get a Google AnalyticsReporting DateRange from
        /// DateTimeOffset values for start and end dates.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        DateRange DateRangeFromBounds(DateTimeOffset startDate, DateTimeOffset endDate);

        /// <summary>
        /// <para>
        /// Get a Google AnalyticsReporting DateRange from
        /// DateTimeOffset or correctly formatted string values for start and end dates.
        /// </para>
        /// <para>Strings should be dates in the ISO-8601 short form ("yyyy-MM-dd").</para>
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        DateRange DateRangeFromBounds(string startDate, DateTimeOffset endDate);

        /// <summary>
        /// <para>
        /// Get a Google AnalyticsReporting DateRange from
        /// DateTimeOffset or correctly formatted string values for start and end dates.
        /// </para>
        /// <para>Strings should be dates in the ISO-8601 short form ("yyyy-MM-dd").</para>
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        DateRange DateRangeFromBounds(DateTimeOffset startDate, string endDate);

        /// <summary>
        /// <para>
        /// Get a Google AnalyticsReporting DateRange from
        /// correctly formatted string values for start and end dates.
        /// </para>
        /// <para>Strings should be dates in the ISO-8601 short form ("yyyy-MM-dd").</para>
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        DateRange DateRangeFromBounds(string startDate, string endDate);

        /// <summary>
        /// <para>Fetch data for all Biobanks from the configured Google Analytics Reporting view, within the specified date ranges.</para>
        /// <para>The data is subsequently written to the local database.</para>
        /// </summary>
        /// <param name="dateRanges"></param>
        Task DownloadAllBiobankData(IList<DateRange> dateRanges);

        /// <summary>
        /// <para>Fetch Driectory level analytics data from the configured Google Analytics Reporting view, within the specified date ranges.</para>
        /// <para>The data is subsequently written to the local database.</para>
        /// </summary>
        /// <param name="dateRanges"></param>
        Task DownloadDirectoryData(IList<DateRange> dateRanges);
    }
}