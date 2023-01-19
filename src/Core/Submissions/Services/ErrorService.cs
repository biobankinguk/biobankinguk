using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;
using Core.Submissions.Exceptions;
using Core.Submissions.Services.Contracts;
using Core.Submissions.Types;

namespace Core.Submissions.Services
{
    /// <inheritdoc />
    public class ErrorService : IErrorService
    {
        private readonly ApplicationDbContext _db;

        /// <inheritdoc />
        public ErrorService(ApplicationDbContext db)
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

        /// <inheritdoc />
        public async Task Add(int submissionId, Operation op, string type, ICollection<BiobanksValidationResult> messages, int biobankId)
        {
            var sub = await _db.Submissions
                .Where(x => x.Id == submissionId)
                .Include(x => x.Errors)
                .SingleOrDefaultAsync();

            foreach (var message in messages)
            {
                sub.Errors.Add(new Error
                {
                    Message = $"Failed to {op} {type}: {message.ErrorMessage}",
                    RecordIdentifiers = message.RecordIdentifiers,
                    SubmissionId = submissionId
                });
            }

            await _db.SaveChangesAsync();
        }
    }
}