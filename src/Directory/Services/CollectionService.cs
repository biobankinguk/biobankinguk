using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly IBiobankReadService _readService;
        private readonly IBiobankIndexService _indexService;

        private readonly BiobanksDbContext _db;

        public CollectionService(
            BiobanksDbContext db,
            IBiobankReadService readService,
            IBiobankIndexService indexService)
        {
            _db = db;
            _readService = readService;
            _indexService = indexService;
        }

        /// <summary>
        /// Delete the Collection with a given Id. Will only proceed if the Collection has no SampleSets.
        /// </summary>
        /// <param name="id">The Id of the Collection to delete</param>
        public async Task<bool> DeleteCollection(int id)
        {
            if (await HasSampleSets(id))
                return false;

            var collection = new Collection { OrganisationId = id };
            _db.Collections.Attach(collection);
            _db.Collections.Remove(collection);

            await _db.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Add a new Collection
        /// </summary>
        public async Task<Collection> AddCollection(Collection collection)
        {
            // Update Timestamp
            collection.LastUpdated = DateTime.Now;

            _db.Collections.Add(collection);

            await _db.SaveChangesAsync();

            return collection;
        }

        /// <summary>
        /// Update an exisiting Collection.
        /// </summary>
        /// <param name="collection">The updated Collection</param>
        public async Task<Collection> UpdateCollection(Collection collection)
        {
            var currentCollection = await _db.Collections
                .Include(x => x.AssociatedData)
                .Include(x => x.ConsentRestrictions)
                .FirstOrDefaultAsync(x => x.CollectionId == collection.CollectionId);

            if (currentCollection != null)
            {
                currentCollection.AssociatedData.Clear();
                currentCollection.ConsentRestrictions.Clear();

                currentCollection.AssociatedData = collection.AssociatedData;
                currentCollection.ConsentRestrictions = collection.ConsentRestrictions;

                currentCollection.OntologyTermId = collection.OntologyTermId;
                currentCollection.Title = collection.Title;
                currentCollection.Description = collection.Description;
                currentCollection.StartDate = collection.StartDate;
                currentCollection.AccessConditionId = collection.AccessConditionId;
                currentCollection.CollectionTypeId = collection.CollectionTypeId;
                currentCollection.CollectionStatusId = collection.CollectionStatusId;
                currentCollection.LastUpdated = DateTime.Now;

                await _db.SaveChangesAsync();

                if (!await _readService.IsCollectionBiobankSuspendedAsync(collection.CollectionId))
                    await _indexService.UpdateCollectionDetails(collection.CollectionId);
            }

            return collection;
        }

        /// <summary>
        /// Get the untracked Collection with associated Collection Id.
        /// </summary>
        /// <param name="id">The Id of the Collecton</param>
        /// <returns>The collection with the given Id, or null if no collection exists with that Id</returns>
        public async Task<Collection> GetCollection(int id)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.AccessCondition)
                .Include(x => x.AssociatedData)
                .Include(x => x.CollectionType)
                .Include(x => x.CollectionStatus)
                .Include(x => x.ConsentRestrictions)
                .Include(x => x.OntologyTerm)
                .FirstOrDefaultAsync(x => x.CollectionId == id);

        /// <summary>
        /// Get the untracked Collection with associated Collection Id, including all SampleSets and MaterialDetails
        /// </summary>
        /// <param name="id">The Id of the Collecton</param>
        /// <returns>The collection with the given Id, or null if no collection exists with that Id</returns>
        public async Task<Collection> GetEntireCollection(int id)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.AccessCondition)
                .Include(x => x.AssociatedData)
                .Include(x => x.AssociatedData.Select(y => y.AssociatedDataType))
                .Include(x => x.AssociatedData.Select(y => y.AssociatedDataProcurementTimeframe))
                .Include(x => x.CollectionType)
                .Include(x => x.CollectionStatus)
                .Include(x => x.ConsentRestrictions)
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleSets)
                .Include(x => x.SampleSets.Select(y => y.AgeRange))
                .Include(x => x.SampleSets.Select(y => y.Sex))
                .Include(x => x.SampleSets.Select(y => y.MaterialDetails.Select(z => z.MaterialType)))
                .Include(x => x.SampleSets.Select(y => y.MaterialDetails.Select(z => z.StorageTemperature)))
                .FirstOrDefaultAsync(x => x.CollectionId == id);

        /// <summary>
        /// Get the untracked Collection with associated Collection Id, with all relevant properites for indexing.
        /// </summary>
        /// <param name="id">The Id of the Collecton</param>
        /// <returns>The collection with the given Id, or null if no collection exists with that Id</returns>
        public async Task<Collection> GetIndexableCollection(int id)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.AccessCondition)
                .Include(x => x.AssociatedData)
                .Include(x => x.AssociatedData.Select(y => y.AssociatedDataType))
                .Include(x => x.AssociatedData.Select(y => y.AssociatedDataProcurementTimeframe))
                .Include(x => x.CollectionType)
                .Include(x => x.CollectionStatus)
                .Include(x => x.ConsentRestrictions)
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleSets)
                .FirstOrDefaultAsync(x => x.CollectionId == id);

        /// <summary>
        /// List untracked Collections
        /// </summary>
        /// <param name="organisationId">Optional filter by Organistation Id</param>
        /// <returns>Enumerable of untracked Collections</returns>
        public async Task<IEnumerable<Collection>> ListCollections(int organisationId = default)
            => await _db.Collections
                .AsNoTracking()
                .Include(x => x.OntologyTerm)
                .Include(x => x.SampleSets.Select(y => y.MaterialDetails))
                .Where(x => x.OrganisationId == organisationId || organisationId == default)
                .ToListAsync();

        /// <summary>
        /// List untracked Collections, with given OntologyTerm value.
        /// </summary>
        /// <param name="ontologyTerm">Value of the OntologyTerm to filter by</param>
        /// <returns>Enumerable of untracked Collections</returns>
        public async Task<IEnumerable<Collection>> ListCollectionsByOntologyTerm(string ontologyTerm)
            => await _db.Collections
                .AsNoTracking()
                .Where(x => x.OntologyTerm.Value == ontologyTerm)
                .ToListAsync();

        /// <summary>
        /// Whether the Collection has been created from aggreagted data submitted via the API.
        /// </summary>
        /// <param name="id">The Id of the Collecton</param>
        public async Task<bool> IsFromApi(int id)
            => await _db.Collections.AnyAsync(x => x.CollectionId == id && x.FromApi);

        /// <summary>
        /// Check if the Collection has associated SampleSets
        /// </summary>
        /// <param name="id">The Id of the Collection</param>
        public async Task<bool> HasSampleSets(int id)
            => await _db.SampleSets.AnyAsync(x => x.CollectionId == id);
    }
}
