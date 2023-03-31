using Biobanks.Analytics.Services.Contracts;
using Biobanks.Data;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Analytics;

namespace Biobanks.Analytics.Services
{
    /// <inheritdoc />
    public class AnalyticsService : IAnalyticsService
    {
        private readonly ApplicationDbContext _db;

        public AnalyticsService(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OrganisationAnalytic>> GetOrganisationAnalytics(DateTimeOffset startDate, DateTimeOffset endDate)
            => await _db.OrganisationAnalytics.AsNoTracking()
                .Where(x => x.Date >= startDate && x.Date <= endDate)
                .ToListAsync();

        /// <inheritdoc />
        public async Task<IEnumerable<DirectoryAnalyticEvent>> GetAnalyticsEvents(DateTimeOffset startDate, DateTimeOffset endDate)
            => await _db.DirectoryAnalyticEvents.AsNoTracking()
                .Where(x => x.Date >= startDate && x.Date <= endDate)
                .ToListAsync();

        /// <inheritdoc />
        public async Task<IEnumerable<DirectoryAnalyticMetric>> GetAnalyticsMetrics(DateTimeOffset startDate, DateTimeOffset endDate)
            => await _db.DirectoryAnalyticMetrics.AsNoTracking()
                .Where(x => x.Date >= startDate && x.Date <= endDate)
                .ToListAsync();

        /// <inheritdoc />
        public async Task<DateTimeOffset> GetLatestOrganisationAnalyticsTimestamp()
            => (await _db.OrganisationAnalytics.ToListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTimeOffset.MinValue).Max();

        /// <inheritdoc />
        public async Task<DateTimeOffset> GetLatestAnalyticsEventTimestamp()
            => (await _db.DirectoryAnalyticEvents.ToListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTimeOffset.MinValue).Max();

        /// <inheritdoc />
        public async Task<DateTimeOffset> GetLatestAnalyticsMetricTimestamp()
            => (await _db.DirectoryAnalyticMetrics.ToListAsync()).Select(x => x.Date).DefaultIfEmpty(DateTimeOffset.MinValue).Max();
    }
}
