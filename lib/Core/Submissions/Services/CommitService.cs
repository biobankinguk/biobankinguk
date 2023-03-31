using System;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Submissions.Consts;
using Biobanks.Submissions.Services.Contracts;
using Biobanks.Submissions.Types;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Submissions.Services
{
    /// <inheritdoc />
    public class CommitService : ICommitService
    {
        private readonly ApplicationDbContext _db;

        /// <inheritdoc />
        public CommitService(ApplicationDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task CommitStagedData(bool replace, int organisationId)
        {
            await _db.Database.BeginTransactionAsync();

            // if replacing, delete everything from the live areas first
            if (replace)
            {
                await _db.Database.ExecuteSqlRawAsync(SqlConsts.DeleteAllLiveDiagnoses(organisationId));
                // soft deletes samples from live by setting IsDirty and IsDeleted = true
                await _db.Database.ExecuteSqlRawAsync(SqlConsts.DeleteAllLiveSamples(organisationId));
                await _db.Database.ExecuteSqlRawAsync(SqlConsts.DeleteAllLiveTreatments(organisationId));
            }

            // merge staged entities into corresponding live tables
            await _db.Database.ExecuteSqlRawAsync(SqlConsts.MergeStagedDiagnosesIntoLive(organisationId));
            await _db.Database.ExecuteSqlRawAsync(SqlConsts.MergeStagedSamplesIntoLive(organisationId));
            await _db.Database.ExecuteSqlRawAsync(SqlConsts.MergeStagedTreatmentsIntoLive(organisationId));

            _db.Database.CommitTransaction();

            // mark the open submissions as committed
            var committedStatus = _db.Statuses.FirstOrDefault(s => s.Value == Statuses.Committed);

            foreach (var submission in _db.Submissions
                .Include(s => s.Status)
                .Where(s => s.BiobankId == organisationId && s.Status.Value == Statuses.Open))
            {
                submission.StatusChangeTimestamp = DateTime.Now;
                submission.Status = committedStatus;
            }

            await _db.SaveChangesAsync();
        }
    }
}
