﻿using Google.Apis.AnalyticsReporting.v4.Data;

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
        /// <para>Fetch data for all Biobanks from the configured Google Analytics Reporting view, within the specified date range.</para>
        /// <para>The data is subsequently written to the local database.</para>
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        Task DownloadAllBiobankData(DateTimeOffset startDate, DateTimeOffset endDate);

        /// <summary>
        /// <para>Fetch Directory level analytics data from the configured Google Analytics Reporting view, within the specified date range.</para>
        /// <para>The data is subsequently written to the local database.</para>
        /// </summary>
        /// <param name="dateRanges"></param>
        Task DownloadDirectoryData(DateTimeOffset startDate, DateTimeOffset endDate);
    }
}