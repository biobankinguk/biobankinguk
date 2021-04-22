using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Data;
using Biobanks.Submissions.Core.Consts;
using Biobanks.Submissions.Core.Services.Contracts;
using Biobanks.Submissions.Core.Types;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Biobanks.Submissions.Core.Services
{
    /// <inheritdoc />
    public class CommitService : ICommitService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public CommitService(BiobanksDbContext db)
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
            foreach (var submission in _db.Submissions
                .Include(s => s.Status)
                .Where(s => s.BiobankId == organisationId && s.Status.Value == Statuses.Open))
            {
                submission.StatusChangeTimestamp = DateTime.Now;
                submission.Status = _db.Statuses.FirstOrDefault(s => s.Value == Statuses.Committed);
            }

            await _db.SaveChangesAsync();
        }
    }
}