using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Types;
using Biobanks.Submissions.Core.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;
using LinqKit;

namespace Biobanks.Submissions.Core.Services
{
    /// <inheritdoc />
    public class SubmissionService : ISubmissionService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public SubmissionService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<Submission> Get(int submissionId)
            => await _db.Submissions
                .AsNoTracking()
                .Include(x => x.Status)
                .Include(x => x.Errors)
                .SingleOrDefaultAsync(x => x.Id == submissionId);

        /// <inheritdoc />
        public async Task<(int total, IEnumerable<Submission> submissions)>
            List(int biobankId, SubmissionPaginationParams paging)
        {
            // we're gonna build up conditional stuff on this query, so store a basic one for now
            var query = _db.Submissions.AsNoTracking();

            var predicate = PredicateBuilder.New<Submission>(x => x.BiobankId == biobankId); //always filter on bb id

            DateTime? since = null; //so we can abuse since mechanics WITHOUT modifying the paging object

            if (paging.N > 0 && paging.Since == null) //since precedes n
            {
                //last n commits essentially abuses the since logic:
                //we set since to the earliest timestamp of the nth last commit
                //and then proceed like we would for a regular since query

                //get the timestamp of the nth most recent commit
                var nthCommitTimeStamp = (await query
                        .Where(predicate.And(y => y.Status.Value == Statuses.Committed))
                        .OrderByDescending(x => x.SubmissionTimestamp)
                        .Select(x => new { x.StatusChangeTimestamp, x.SubmissionTimestamp })
                        .Distinct()
                        .Take(paging.N)
                        .ToListAsync()) //Execute SQL here so the ordering works how we expect?
                    .LastOrDefault()
                    ?.StatusChangeTimestamp;

                //set since to the earliest submission timestamp in that commit
                if (nthCommitTimeStamp != null)
                {
                    since = await query
                    .Where(x => x.StatusChangeTimestamp == nthCommitTimeStamp)
                        .Select(x => x.SubmissionTimestamp)
                        .FirstOrDefaultAsync();
                }

                //if we had no success, "since" will be set to DateTime's default initial value
                //which is kinda useless to us
                //so we should drop it
                if (since == new DateTime()) since = null;
            }

            if (paging.Since != null) since = paging.Since;

            if (since != null) predicate = predicate.And(x => x.SubmissionTimestamp > since);

            query = query.Where(predicate); //add the filter permanently now

            return (await query.CountAsync(),
                await query
                    .OrderByDescending(x => x.SubmissionTimestamp)
                    .Include(x => x.Status)
                    .Include(x => x.Errors)
                    .Skip(paging.Offset)
                    .Take(paging.Limit)
                    .ToListAsync());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Submission>> ListSubmissionsInProgress(int biobankId)
            => await _db.Submissions.Where(s =>
                s.BiobankId == biobankId
                && s.Status.Value == Statuses.Open
                && s.RecordsProcessed != s.TotalRecords)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<Submission> CreateSubmission(int totalRecords, int biobankId)
        {
            var status = await _db.Statuses
                .Where(x => x.Value == Statuses.Open)
                .SingleAsync();

            var sub = new Submission
            {
                BiobankId = biobankId,
                TotalRecords = totalRecords,
                Status = status
            };

            await _db.Submissions.AddAsync(sub);

            await _db.SaveChangesAsync();

            return sub;
        }

        /// <inheritdoc />
        public async Task DeleteSubmission(int submissionId)
        {
            var submission = await _db.Submissions.SingleAsync(s => s.Id == submissionId);
            _db.Remove(submission);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task ProcessRecords(int submissionId, int n)
        {
            var sub = await _db.Submissions.Where(x => x.Id == submissionId).SingleOrDefaultAsync();

            if (sub == null) throw new KeyNotFoundException();

            var newTotalRecordsProcessed = sub.RecordsProcessed + n;
            sub.RecordsProcessed = newTotalRecordsProcessed <= sub.TotalRecords ? newTotalRecordsProcessed : sub.TotalRecords;
            sub.StatusChangeTimestamp = DateTime.Now;

            await _db.SaveChangesAsync();
        }
    }
}