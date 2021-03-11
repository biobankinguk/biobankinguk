using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Api.Types;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class ErrorService : IErrorService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public ErrorService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<(int biobankId, int total, IEnumerable<Error> errors)>
            List(int submissionId, PaginationParams paging)
        {
            var biobankId = await _db.Submissions
                .AsNoTracking()
                .Where(x => x.Id == submissionId)
                .Select(x => x.BiobankId)
                .SingleOrDefaultAsync();

            var errors = _db.Errors
                .AsNoTracking()
                .Where(x => x.SubmissionId == submissionId);

            return (
                biobankId,
                await errors.CountAsync(),
                await errors.Skip(paging.Offset).Take(paging.Limit).ToListAsync());
        }

        /// <inheritdoc />
        public async Task<Error> Get(int errorId)
            => await _db.Errors
                .AsNoTracking()
                .Include(x => x.Submission)
                .Where(x => x.Id == errorId)
                .SingleOrDefaultAsync();
    }
}