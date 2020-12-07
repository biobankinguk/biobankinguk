using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Common.Data;
using Biobanks.Common.Types;
using Biobanks.SubmissionExpiryJob.Services.Contracts;
using Biobanks.SubmissionExpiryJob.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Z.EntityFramework.Plus;

namespace Biobanks.SubmissionExpiryJob.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly SubmissionsDbContext _db;
        private readonly SettingsModel _settings;

        public SubmissionService(SubmissionsDbContext db, IOptions<SettingsModel> settings)
        {
            _db = db;
            _settings = settings.Value;
        }

        public async Task<IEnumerable<int>> GetOrganisationsWithExpiringSubmissions()
        {
            var expiry = DateTime.Now.Subtract(TimeSpan.FromDays(_settings.ExpiryDays));

            return await _db.Submissions
                    .Where(s => s.StatusChangeTimestamp < expiry)
                    .Select(s => s.BiobankId)
                    .Distinct()
                    .ToListAsync();
        } 

        public async Task ExpireSubmissions(int organisationId)
        {
            // Remove All of Submitted Data
            await _db.StagedDiagnoses.Where(sd => sd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedSamples.Where(ss => ss.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedTreatments.Where(st => st.OrganisationId == organisationId).DeleteAsync();

            // Remove All of Submitted Deletes
            await _db.StagedDiagnosisDeletes.Where(sdd => sdd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedSampleDeletes.Where(ssd => ssd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedTreatmentDeletes.Where(std => std.OrganisationId == organisationId).DeleteAsync();

            // Mark Submissions As Rejected
            foreach (var submission in _db.Submissions.Where(s => s.BiobankId == organisationId))
            {
                submission.StatusChangeTimestamp = DateTime.Now;
                submission.Status = await _db.Statuses.FirstOrDefaultAsync(s => s.Value == Statuses.Rejected);
            }

            await _db.SaveChangesAsync();
        }
    }
}
