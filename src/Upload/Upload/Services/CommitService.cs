using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Upload;
using Upload.Common.Data;
using Upload.Common.Types;
using Upload.Contracts;

namespace Upload.Services
{
    /// <inheritdoc />
    public class CommitService : ICommitService
    {

        private readonly UploadContext _db;

        /// <inheritdoc />
        public CommitService(UploadContext db)
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
                await _db.Database.ExecuteSqlRawAsync(SqlConsts.DeleteAllLiveSamples(organisationId));
                await _db.Database.ExecuteSqlRawAsync(SqlConsts.DeleteAllLiveTreatments(organisationId));
            }

            // merge staged entities into corresponding live tables
            await _db.Database.ExecuteSqlRawAsync(SqlConsts.MergeStagedDiagnosesIntoLive(organisationId));
            await _db.Database.ExecuteSqlRawAsync(SqlConsts.MergeStagedSamplesIntoLive(organisationId));
            await _db.Database.ExecuteSqlRawAsync(SqlConsts.MergeStagedTreatmentsIntoLive(organisationId));

            _db.Database.CommitTransaction();

            //TODO need to do UploadStatus lookup - or move to DB?
            // mark the open submissions as committed
            foreach (var submission in _db.Submissions
                .Include(s => s.UploadStatus)
                .Where(s => s.OrganisationId == organisationId && s.UploadStatus == Statuses.Open))
            {
                submission.StatusChangeTimestamp = DateTime.Now;
                submission.UploadStatus = Statuses.Committed;
            }

            await _db.SaveChangesAsync();
        }
    }
}