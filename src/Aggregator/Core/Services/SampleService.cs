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

        public async Task<IEnumerable<LiveSample>> ListSimilarSamples(LiveSample sample)
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

        public async Task<IEnumerable<LiveSample>> ListDirtySamples()
            => await _db.Samples.Where(x => x.IsDirty).ToListAsync();

        public async Task CleanSamples(IEnumerable<LiveSample> samples)
            => await _db.Samples
                    .Where(x => samples.Select(x => x.Id).Contains(x.Id))
                    .UpdateFromQueryAsync(x => new LiveSample { IsDirty = false });

        public async Task DeleteFlaggedSamples()
            => await _db.Samples.Where(x => x.IsDeleted).DeleteAsync();

    }
}
