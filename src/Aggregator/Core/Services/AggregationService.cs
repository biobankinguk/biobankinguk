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

        public async Task<IEnumerable<Collection>> GroupSamples(IEnumerable<LiveSample> samples)
        {
            // Currently Only Supports Extracted Samples
            var extractedMaterialGroups = await _db.MaterialTypeGroups.Where(x => x.Value.StartsWith("Extracted")).ToListAsync();
            var extractedSamples = samples.Where(x => x.MaterialType.MaterialTypeGroups.Any(x => extractedMaterialGroups.Contains(x));

            // Grouping Of Samples
            var collections = extractedSamples.GroupBy(x => 
                new 
                { 
                    x.OrganisationId, 
                    x.CollectionName,
                    x.SampleContentId
                })
                .Select(group => group.First())
                .Select(sample => 
                {
                    var collection = _db.Collections.FirstOrDefault(x =>
                        x.OrganisationId == sample.OrganisationId &&
                        x.Title == sample.CollectionName &&
                        x.FromApi
                    );

                    return collection ?? new Collection
                    {
                        OrganisationId = sample.OrganisationId,
                        Title = sample.CollectionName,
                        FromApi = true
                    };
                });

            return collections;
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

        public async Task AddCollectionAsync(Collection collection)
            => await _db.Collections.AddAsync(collection);

        public async Task UpdateCollectionAsync(Collection collection)
        {
            _db.Update(collection);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCollectionAsync(Collection collection)
            => await _db.Collections.Where(x => x.CollectionId == collection.CollectionId).DeleteAsync();

        public async Task DeleteFlaggedSamplesAsync()
            => await _db.Samples.Where(x => x.IsDeleted).DeleteAsync();

    }
}
