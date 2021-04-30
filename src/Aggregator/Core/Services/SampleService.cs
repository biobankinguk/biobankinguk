using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Core.Services
{
    public class SampleService : ISampleService
    {
        private readonly BiobanksDbContext _db;

        public SampleService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LiveSample>> ListSimilarSamplesAsync(LiveSample sample)
        {
            return await _db.Samples
                .Include(x => x.SampleContent)
                .Include(x => x.SampleContentMethod)
                .Where(x =>
                    x.OrganisationId == sample.OrganisationId &&
                    x.CollectionName == sample.CollectionName
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<LiveSample>> ListDirtySamplesAsync()
            => await _db.Samples.Where(x => x.IsDirty).ToListAsync();

        public async Task CleanSamplesAsync(IEnumerable<LiveSample> samples)
            => await _db.Samples
                    .Where(x => samples.Select(x => x.Id).Contains(x.Id))
                    .UpdateFromQueryAsync(x => new LiveSample { IsDirty = false });

        public async Task DeleteFlaggedSamplesAsync()
            => await _db.Samples.Where(x => x.IsDeleted).DeleteAsync();

        // Moved to its own specific service class in future?
        public async Task DeleteSampleSetById(int id)
            => await _db.SampleSets.Where(x => x.Id == id).DeleteAsync();

    }
}
