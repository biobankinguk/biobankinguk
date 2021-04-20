using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Core.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly BiobanksDbContext _db;

        public AggregationService(BiobanksDbContext db)
        {
            _db = db;
        }

        public Task<IEnumerable<Collection>> GroupByCollectionsAsync(IEnumerable<LiveSample> samples)
            => throw new System.NotImplementedException();

        public Task<IEnumerable<LiveSample>> ListCollectionSamplesAsync(Collection collection)
            => throw new System.NotImplementedException();

        public async Task<IEnumerable<LiveSample>> ListDirtySamplesAsync()
            => await _db.Samples.Where(x => x.IsDirty).ToListAsync();

        public async Task DeleteFlaggedSamplesAsync()
            => await _db.Samples.Where(x => x.IsDeleted).DeleteAsync();

        public async Task DeleteCollectionAsync(Collection collection)
            => await _db.Collections.Where(x => x.CollectionId == collection.CollectionId).DeleteAsync();
    }
}
