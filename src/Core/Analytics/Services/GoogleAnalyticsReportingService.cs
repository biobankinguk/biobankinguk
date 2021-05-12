using Google.Apis.AnalyticsReporting.v4.Data;

using System;

namespace Biobanks.Analytics.Services
{
    /// <inheritdoc />
    public class GoogleAnalyticsReportingService : IGoogleAnalyticsReportingService
    {
        private const string _gaDateRangeFormat = "yyyy-MM-dd";

        /// <inheritdoc />
        public DateRange PeriodAsDateRange(int year, int endQuarter, int reportPeriod)
        {
            const int monthsPerQuarter = 3;
            var month = endQuarter * monthsPerQuarter;
            var lastDayofQuarter = DateTime.DaysInMonth(year, month);

            var endDate = new DateTimeOffset(year, month, lastDayofQuarter, 0, 0, 0, TimeSpan.Zero);
            //get start date by subtracting report period (specified in quarters) from end date
            var startDate = endDate.AddMonths(-1 * reportPeriod * monthsPerQuarter).AddDays(1);

            return new()
            {
                StartDate = startDate.ToString(_gaDateRangeFormat),
                EndDate = endDate.ToString(_gaDateRangeFormat)
            };
        }

        /// <inheritdoc />
        public (DateTimeOffset startDate, DateTimeOffset endDate) GetDateRangeBounds(DateRange dateRange)
            => (DateTimeOffset.Parse(dateRange.StartDate), DateTimeOffset.Parse(dateRange.EndDate));
    }
}
