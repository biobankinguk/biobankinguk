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
    public class SampleService : ISampleService
    {
        private readonly BiobanksDbContext _db;

        public SampleService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LiveSample>> ListRelevantSamplesAsync(Collection collection)
        {
            return await _db.Samples
                .Where(x =>
                    x.OrganisationId == collection.OrganisationId &&
                    x.CollectionName == collection.Title
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<LiveSample>> ListDirtySamplesAsync()
            => await _db.Samples.Where(x => x.IsDirty).ToListAsync();

        public async Task DeleteFlaggedSamplesAsync()
            => await _db.Samples.Where(x => x.IsDeleted).DeleteAsync();

    }
}
