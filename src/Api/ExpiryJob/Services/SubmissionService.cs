using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Common.Data;
using Biobanks.Common.Types;
using Biobanks.ExpiryJob.Services.Contracts;
using Biobanks.ExpiryJob.Settings;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.ExpiryJob.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly SubmissionsDbContext _db;
        private readonly SettingsModel _settings;

        public SubmissionService(SubmissionsDbContext db, SettingsModel settings)
        {
            _db = db;
            _settings = settings;
        }

        public async Task<IEnumerable<int>> GetOrganisationsWithExpiringSubmissions()
        => await _db.Submissions.GroupBy(s => s.BiobankId).Select(grp => new
            {
                biobankid = grp.Key,
                lastUpdatedSubmission = grp.OrderByDescending(x => x.StatusChangeTimestamp).FirstOrDefault(lastUpdate =>
                    DateTime.Now
                        .Subtract(TimeSpan.FromDays(_settings.ExpiryDays)) > lastUpdate.StatusChangeTimestamp)
            }).Select(s => s.biobankid).ToListAsync();

        public async Task ExpireSubmissions(int organisationId)
        {
            foreach (var submission in _db.Submissions.Where(s => s.BiobankId == organisationId))
            {
                submission.StatusChangeTimestamp = DateTime.Now;
                submission.Status = await _db.Statuses.FirstOrDefaultAsync(s => s.Value == Statuses.Rejected);
            }

            await _db.SaveChangesAsync();
        }
    }
}
