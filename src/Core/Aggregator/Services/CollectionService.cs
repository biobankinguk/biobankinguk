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
    public class CollectionService : ICollectionService
    {
        private readonly BiobanksDbContext _db;

        public CollectionService(BiobanksDbContext db)
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
            _db.Update(collection);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCollection(int id)
            => await _db.Collections.Where(x => x.CollectionId == id).DeleteAsync();

        public async Task DeleteSampleSetByIds(IEnumerable<int> ids)
        {
            foreach (var sampleSet in _db.SampleSets.Where(x => ids.Contains(x.Id)))
            {
                _db.Remove(sampleSet);
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteMaterialDetailsBySampleSetIds(IEnumerable<int> ids)
        {
            foreach (var materialDetail in _db.MaterialDetails.Where(x => ids.Contains(x.SampleSetId)))
            {
                _db.Remove(materialDetail);
            }

            await _db.SaveChangesAsync();
        }
    }
}
