using System.Collections.Generic;
using System.Threading.Tasks;
using Entities.Api.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class SnomedTagService : ISnomedTagService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public SnomedTagService(SubmissionsDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SnomedTag>> List()
            => await _db.SnomedTags
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc />
        public async Task<SnomedTag> Get(int snomedTagId)
            => await _db.SnomedTags
                .Include(st => st.SnomedTerms)
                .FirstOrDefaultAsync(st => st.Id == snomedTagId);

        /// <inheritdoc />
        public async Task<SnomedTag> GetByValue(string value)
            => await _db.SnomedTags.FirstOrDefaultAsync(st => st.Value == value);

        /// <inheritdoc />
        public async Task<SnomedTag> Create(SnomedTag snomedTag)
        {
            await _db.SnomedTags.AddAsync(snomedTag);
            await _db.SaveChangesAsync();
            return snomedTag;
        }

        /// <inheritdoc />
        public async Task Update(SnomedTag snomedTag)
        {
            _db.SnomedTags.Update(snomedTag);
            await _db.SaveChangesAsync();
        }
    }
}