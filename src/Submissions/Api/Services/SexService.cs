using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class SexService : ISexService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public SexService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Sex>> List()
            => await _db.Sexes.AsNoTracking().ToListAsync();

        /// <inheritdoc />
        public async Task<Sex> Get(int sexId)
            => await _db.Sexes.FirstOrDefaultAsync(s => s.Id == sexId);

        /// <inheritdoc />
        public async Task<Sex> GetByValue(string value)
            => await _db.Sexes.FirstOrDefaultAsync(s => s.Value == value);

        /// <inheritdoc />
        public async Task<Sex> Create(Sex sex)
        {
            await _db.Sexes.AddAsync(sex);
            await _db.SaveChangesAsync();
            return sex;
        }

        /// <inheritdoc />
        public async Task Update(Sex sex)
        {
            _db.Sexes.Update(sex);
            await _db.SaveChangesAsync();
        }
    }
}