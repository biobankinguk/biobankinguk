using Biobanks.Data;
using Core.Submissions.Config;
using Core.Submissions.Services.Contracts;
using Core.Submissions.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Z.EntityFramework.Plus;

namespace Core.Submissions.Services
{
    public class SubmissionExpiryService : ISubmissionExpiryService
    {
        private readonly ApplicationDbContext _db;
        private readonly int _expiryDays = 0;

        public SubmissionExpiryService(ApplicationDbContext db, IOptions<ExpiryConfigModel> config)
        {
            _db = db;
            _expiryDays = config.Value.ExpiryDays;
        }

        public async Task<IEnumerable<int>> GetOrganisationsWithExpiringSubmissions()
        {
            var expiry = DateTime.Now.Subtract(TimeSpan.FromDays(_expiryDays));

            return await _db.Submissions
                .Where(s =>
                    s.Status.Value == Statuses.Open &&
                    s.StatusChangeTimestamp < expiry
                )
                .Select(s => s.BiobankId)
                .Distinct()
                .ToListAsync();
        }

        public async Task ExpireSubmissions(int organisationId)
        {
            // Remove All of Staged Data
            await _db.StagedDiagnoses.Where(sd => sd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedSamples.Where(ss => ss.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedTreatments.Where(st => st.OrganisationId == organisationId).DeleteAsync();

            // Remove All of Staged Deletes
            await _db.StagedDiagnosisDeletes.Where(sdd => sdd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedSampleDeletes.Where(ssd => ssd.OrganisationId == organisationId).DeleteAsync();
            await _db.StagedTreatmentDeletes.Where(std => std.OrganisationId == organisationId).DeleteAsync();

            // Mark Submissions As Expired
            var expiredStatus = await _db.Statuses.FirstOrDefaultAsync(s => s.Value == Statuses.Expired);

            await _db.Submissions
                .Where(s => s.BiobankId == organisationId)
                .Where(s => s.Status.Value == Statuses.Open)
                .ForEachAsync(s =>
                {
                    s.StatusChangeTimestamp = DateTime.Now;
                    s.Status = expiredStatus;
                });

            await _db.SaveChangesAsync();
        }
    }
}
