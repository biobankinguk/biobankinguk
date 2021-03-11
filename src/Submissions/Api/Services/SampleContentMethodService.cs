using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Biobanks.Data;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class SampleContentMethodService : ISampleContentMethodService
    {
        private readonly BiobanksDbContext _db;

        /// <inheritdoc />
        public SampleContentMethodService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SampleContentMethod>> List()
            => await _db.SampleContentMethods.AsNoTracking().ToListAsync();

        /// <inheritdoc />
        public async Task<SampleContentMethod> Get(int sampleContentMethodId)
            => await _db.SampleContentMethods.FirstOrDefaultAsync(scm => scm.Id == sampleContentMethodId);

        /// <inheritdoc />
        public async Task<SampleContentMethod> GetByValue(string value)
            => await _db.SampleContentMethods.FirstOrDefaultAsync(scm => scm.Value == value);

        /// <inheritdoc />
        public async Task<SampleContentMethod> Create(SampleContentMethod sampleContentMethod)
        {
            await _db.SampleContentMethods.AddAsync(sampleContentMethod);
            await _db.SaveChangesAsync();
            return sampleContentMethod;
        }

        /// <inheritdoc />
        public async Task Update(SampleContentMethod sampleContentMethod)
        {
            _db.SampleContentMethods.Update(sampleContentMethod);
            await _db.SaveChangesAsync();
        }
    }
}