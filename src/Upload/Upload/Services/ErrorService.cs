using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.SubmissionApi.Services.Contracts;
using Common.Data;
using Common.Data.Upload;
using Microsoft.EntityFrameworkCore;
using Upload.Common.Types;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class ErrorService : IErrorService
    {
        private readonly UploadContext _db;

        /// <inheritdoc />
        public ErrorService(UploadContext db)
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