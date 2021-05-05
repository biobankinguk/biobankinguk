﻿using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Z.EntityFramework.Plus;

namespace Biobanks.Aggregator.Core.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly BiobanksDbContext _db;

        public CollectionService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<Collection> GetCollectionAsync(int organisationId, string collectionName)
            => await _db.Collections
                .Include(x => x.SampleSets)
                .FirstOrDefaultAsync(x =>
                    x.OrganisationId == organisationId && 
                    x.Title == collectionName && 
                    x.FromApi);

        public async Task AddCollectionAsync(Collection collection)
        {
            await _db.Collections.AddAsync(collection);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateCollectionAsync(Collection collection)
        {
            _db.Update(collection);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteCollectionAsync(Collection collection)
        {
            await _db.Collections.Where(x => x.CollectionId == collection.CollectionId).DeleteAsync();
            await _db.SaveChangesAsync();
        }

        public async Task DeleteSampleSetsByIds(IEnumerable<int> ids)
            => await _db.SampleSets.Where(x => ids.Contains(x.Id)).DeleteAsync();

        public async Task DeleteMaterialDetailsSampleSetIds(IEnumerable<int> ids)
            => await _db.MaterialDetails.Where(x => ids.Contains(x.SampleSetId)).DeleteAsync();
    }
}
