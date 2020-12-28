using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.EntityFrameworkCore;
using Data;

namespace Biobanks.SubmissionApi.Services
{
    /// <inheritdoc />
    public class SampleContentMethodService : ISampleContentMethodService
    {
        private readonly SubmissionsDbContext _db;

        /// <inheritdoc />
        public SampleContentMethodService(SubmissionsDbContext db)
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