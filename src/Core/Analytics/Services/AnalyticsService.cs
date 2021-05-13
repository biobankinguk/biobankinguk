using Biobanks.Analytics.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data.Analytics;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Analytics.Services
{
    /// <inheritdoc />
    public class AnalyticsService : IAnalyticsService
    {
        private readonly BiobanksDbContext _db;

        public AnalyticsService(BiobanksDbContext db)
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

    }
}
