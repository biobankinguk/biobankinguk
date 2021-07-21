using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Biobanks.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Services
{
    public class SampleService : ISampleService
    {
        private readonly BiobanksDbContext _db;

        public SampleService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<LiveSample>> ListSimilarSamples(IEnumerable<LiveSample> samples)
        {
            var sample = samples.First();
            var latest = samples.Max(x => x.DateCreated) + TimeSpan.FromDays(180);
            var earliest = samples.Min(x => x.DateCreated) - TimeSpan.FromDays(180);
            
            return await _db.Samples
                .Include(x => x.SampleContent)
                .Include(x => x.SampleContentMethod)
                .Where(x => x.OrganisationId == sample.OrganisationId)
                .Where(x =>
                    !string.IsNullOrEmpty(sample.SampleContentId)
                        ? x.CollectionName == sample.CollectionName && x.SampleContentId == sample.SampleContentId
                        : x.DateCreated >= earliest && x.DateCreated <= latest
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<LiveSample>> ListDirtyExtractedSamples()
            => await _db.Samples.Where(x => x.IsDirty && !string.IsNullOrEmpty(x.SampleContentId)).ToListAsync();

        public async Task CleanSamples(IEnumerable<LiveSample> samples)
            => await _db.Samples
                    .Where(x => samples.Select(x => x.Id).Contains(x.Id))
                    .UpdateFromQueryAsync(x => new LiveSample { IsDirty = false });

        public async Task DeleteFlaggedSamples()
            => await _db.Samples.Where(x => x.IsDeleted).DeleteAsync();

    }
}
