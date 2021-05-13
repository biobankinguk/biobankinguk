using Biobanks.Entities.Data.Analytics;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Analytics.Services.Contracts
{
    /// <summary>
    /// Service for interacting with Directory Analytics data
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>
        /// Get OrganisationAnalytics within a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        Task<IEnumerable<OrganisationAnalytic>> GetOrganisationAnalytics(DateTimeOffset startDate, DateTimeOffset endDate);

        /// <summary>
        /// Get DirectoryAnalyticsEvents within a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        Task<IEnumerable<DirectoryAnalyticEvent>> GetAnalyticsEvents(DateTimeOffset startDate, DateTimeOffset endDate);

        /// <summary>
        /// Get DirectoryAnalyticsMetrics within a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        Task<IEnumerable<DirectoryAnalyticMetric>> GetAnalyticsMetrics(DateTimeOffset startDate, DateTimeOffset endDate);
    }
}