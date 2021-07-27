using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly BiobanksDbContext _db;

        public CollectionService(BiobanksDbContext db)
        {
            _db = db;
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
        public async Task<bool> IsCollectionFromApi(int id)
            => await _db.Collections.AnyAsync(x => x.CollectionId == id && x.FromApi);

    }
}
