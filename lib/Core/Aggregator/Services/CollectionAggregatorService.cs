using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Services
{
    public class CollectionAggregatorService : ICollectionAggregatorService
    {
        private readonly ApplicationDbContext _db;

        public CollectionAggregatorService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Collection> GetCollection(int organisationId, string collectionName)
            => await _db.Collections
                .Include(x => x.SampleSets)
                .FirstOrDefaultAsync(x =>
                    x.OrganisationId == organisationId && 
                    x.Title == collectionName && 
                    x.FromApi);

        public async Task AddCollection(Collection collection)
        {
            await _db.Collections.AddAsync(collection);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCollection(Collection collection)
        {
            var oldSampleSetIds = _db.SampleSets
                .Where(x => x.CollectionId == collection.CollectionId)
                .Select(x => x.Id)
                .ToList();

            // Delete Old Material Details
            foreach (var materialDetail in _db.MaterialDetails.Where(x => oldSampleSetIds.Contains(x.SampleSetId)))
            {
                _db.Remove(materialDetail);
            }


            // Delete Old SampleSets
            foreach (var sampleSet in _db.SampleSets.Where(x => oldSampleSetIds.Contains(x.Id)))
            {
                _db.Remove(sampleSet);
            }

            await _db.SaveChangesAsync();

            // Update Collection
            _db.Collections.Update(collection);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCollection(int id)
            => await _db.Collections.Where(x => x.CollectionId == id).DeleteAsync();
    }
}
