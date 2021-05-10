using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Api;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            var sample = samples.First();
            var latest = samples.Max(x => x.DateCreated) + TimeSpan.FromDays(180);
            var earliest = samples.Min(x => x.DateCreated) - TimeSpan.FromDays(180);
            
            return await _db.Samples
                .Include(x => x.SampleContent)
                .Include(x => x.SampleContentMethod)
                .Where(x => x.OrganisationId == sample.OrganisationId)
                .Where(x =>
                    !string.IsNullOrEmpty(x.SampleContentId)
                        // Extracted Samples
                        ? x.CollectionName == sample.CollectionName && x.SampleContentId == sample.SampleContentId
                        // Non-Extracted Samples
                        : x.DateCreated >= earliest && x.DateCreated <= latest   
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
