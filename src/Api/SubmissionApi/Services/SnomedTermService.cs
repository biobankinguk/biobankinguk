using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class SnomedTermService : ISnomedTermService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public SnomedTermService(SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SnomedTerm>> List()
            => await _db.SnomedTerms
                .Include(st => st.SnomedTag)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<SnomedTerm> Get(string snomedTermId)
            => await _db.SnomedTerms
                .Include(st => st.SnomedTag)
                .FirstOrDefaultAsync(st => st.Id == snomedTermId);

        /// <inheritdoc />
        public async Task<SnomedTerm> GetByValue(string value)
            => await _db.SnomedTerms.FirstOrDefaultAsync(st => st.Description == value);

        /// <inheritdoc />
        public async Task<SnomedTerm> Create(SnomedTerm snomedTerm)
        {
            await _db.SnomedTerms.AddAsync(snomedTerm);
            await _db.SaveChangesAsync();
            return snomedTerm;
        }

        /// <inheritdoc />
        public async Task Create(IList<SnomedTerm> snomedTerms)
        {
            await _db.SnomedTerms.AddRangeAsync(snomedTerms);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Update(SnomedTerm snomedTerm)
        {
            _db.SnomedTerms.Update(snomedTerm);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task Update(List<SnomedTerm> snomedTerms)
        {
            _db.SnomedTerms.UpdateRange(snomedTerms);
            await _db.SaveChangesAsync();
        }
    }
}