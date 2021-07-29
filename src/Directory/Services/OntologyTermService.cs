using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Contracts;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    /// <inheritdoc/>
    public class OntologyTermService : IOntologyTermService
    {

        private readonly IBiobankIndexService _indexService;

        private readonly BiobanksDbContext _db;

        public OntologyTermService(IBiobankIndexService indexService, BiobanksDbContext db)
        {
            _db = db;
            _indexService = indexService;
        }

        protected IQueryable<OntologyTerm> ReadOnlyQuery(
            string id = null, string value = null, List<string> tags = null, bool onlyDisplayable = false)
        {
            var query = _db.OntologyTerms
                .AsNoTracking()
                .Include(x => x.SnomedTag)
                .Include(x => x.MaterialTypes)
                .Where(x => x.DisplayOnDirectory || !onlyDisplayable);

            // Filter By ID
            if (!string.IsNullOrEmpty(id))
                query = query.Where(x => x.Id == id);

            // Filter By Description
            if (!string.IsNullOrEmpty(value))
                query = query.Where(x => x.Value.Contains(value));

            // Filter By SnomedTag
            if (tags != null)
                query = query.Where(x =>
                    tags.Any()
                        ? x.SnomedTag != null && tags.Contains(x.SnomedTag.Value) // Term With Included Tag
                        : x.SnomedTag == null); // Terms Without Tags

            return query;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OntologyTerm>> List(
            string value = null, List<string> tags = null, bool onlyDisplayable = false)
            => await ReadOnlyQuery(id: null, value, tags, onlyDisplayable).ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<OntologyTerm>> ListPaginated(
            int skip, int take, string value = null, List<string> tags = null, bool onlyDisplayable = false)
        {
            return await ReadOnlyQuery(id: null, value, tags, onlyDisplayable)
                    .OrderByDescending(x => x.DisplayOnDirectory).ThenBy(x => x.Value)
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<OntologyTerm> Get(string id = null, string value = null, List<string> tags = null, bool onlyDisplayable = false)
            => await ReadOnlyQuery(id, value, tags, onlyDisplayable).SingleOrDefaultAsync();

        /// <inheritdoc/>
        public async Task<int> Count(string value = null, List<string> tags = null)
            => await ReadOnlyQuery(id: null, value, tags).CountAsync();

        /// <inheritdoc/>
        public async Task<int> CountCollectionCapabilityUsage(string ontologyTermId)
            => await _db.Collections.CountAsync(x => x.OntologyTermId == ontologyTermId) 
             + await _db.DiagnosisCapabilities.CountAsync(x => x.OntologyTermId == ontologyTermId);

        /// <inheritdoc/>
        public async Task<bool> Exists(string id = null, string value = null, List<string> tags = null)
            => await ReadOnlyQuery(id, value, tags).AnyAsync();

        /// <inheritdoc/>
        public async Task<bool> IsInUse(string id)
            => (await CountCollectionCapabilityUsage(id) > 0);

        /// <inheritdoc/>
        public async Task<OntologyTerm> Create(OntologyTerm ontologyTerm)
        {
            // Add New OntologyTerm
            ontologyTerm = _db.OntologyTerms.Add(ontologyTerm);

            // Link To Material Types
            if (ontologyTerm.MaterialTypes != null)
            {
                var ids = ontologyTerm.MaterialTypes.Select(x => x.Id);

                await _db.MaterialTypes
                    .Where(x => ids.Contains(x.Id))
                    .ForEachAsync(x => x.ExtractionProcedures.Add(ontologyTerm));
            }

            await _db.SaveChangesAsync();

            // Return OntologyTerm With Idenity ID
            return ontologyTerm;
        }

        /// <inheritdoc/>
        public async Task<OntologyTerm> Update(OntologyTerm ontologyTerm)
        {
            var currentTerm = await _db.OntologyTerms.FirstAsync(x => x.Id == ontologyTerm.Id);

            currentTerm.Value = ontologyTerm.Value;
            currentTerm.OtherTerms = ontologyTerm.OtherTerms;
            currentTerm.DisplayOnDirectory = ontologyTerm.DisplayOnDirectory;
            currentTerm.MaterialTypes = ontologyTerm.MaterialTypes;
            currentTerm.SnomedTag = ontologyTerm.SnomedTag;

            await _db.SaveChangesAsync();

            await _indexService.UpdateCollectionsOntologyOtherTerms(ontologyTerm.Value);
            await _indexService.UpdateCapabilitiesOntologyOtherTerms(ontologyTerm.Value);

            // TODO: Check If Manual Update Of Material Types Is Required
            //var Term = (await _ontologyTermRepository.ListAsync(true, x => x.Id == ontologyTerm.Id, null, x => x.MaterialTypes)).FirstOrDefault();
            //var materialTypes = (await _materialTypeRepository.ListAsync(true, x => materialTypeIds.Contains(x.Id))).ToList();
            //Term.MaterialTypes = materialTypes;

            return currentTerm;
        }

        /// <inheritdoc/>
        public async Task Delete(string id)
        {
            var ontologyTerm = new OntologyTerm { Id = id };

            _db.OntologyTerms.Attach(ontologyTerm);
            _db.OntologyTerms.Remove(ontologyTerm);

            await _db.SaveChangesAsync();
        }
    }
}
