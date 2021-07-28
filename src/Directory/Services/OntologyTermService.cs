using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class OntologyTermService : IOntologyTermService
    {

        private readonly BiobanksDbContext _db;

        public OntologyTermService(BiobanksDbContext db)
        {
            _db = db;
        }

        protected IQueryable<OntologyTerm> QueryOntologyTerms(
            string id = null, string description = null, List<string> tags = null, bool onlyDisplayable = false)
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
            if (!string.IsNullOrEmpty(description))
                query = query.Where(x => x.Value.Contains(description));

            // Filter By SnomedTag
            if (tags != null)
                query = query.Where(x =>
                    tags.Any()
                        ? x.SnomedTag != null && tags.Contains(x.SnomedTag.Value) // Term With Included Tag
                        : x.SnomedTag == null); // Terms Without Tags

            return query;
        }

        public async Task<IEnumerable<OntologyTerm>> ListOntologyTerms(
            string description = null, List<string> tags = null, bool onlyDisplayable = false)
            => await QueryOntologyTerms(id: null, description, tags, onlyDisplayable).ToListAsync();

        public async Task<IEnumerable<OntologyTerm>> PaginateOntologyTerms(
            int start, int length, string description = null, List<string> tags = null)
        {
            return await QueryOntologyTerms(id: null, description, tags)
                    .OrderByDescending(x => x.DisplayOnDirectory).ThenBy(x => x.Value)
                    .Skip(start)
                    .Take(length)
                    .ToListAsync();
        }

        public async Task<OntologyTerm> GetOntologyTerm(string id = null, string description = null, List<string> tags = null, bool onlyDisplayable = false)
            => await QueryOntologyTerms(id, description, tags, onlyDisplayable).SingleOrDefaultAsync();

        public async Task<int> CountOntologyTerms(string description = null, List<string> tags = null)
            => await QueryOntologyTerms(id: null, description, tags).CountAsync();

        public async Task<int> GetOntologyTermCollectionCapabilityCount(string id)
            => await _db.Collections.CountAsync(x => x.OntologyTermId == id) 
             + await _db.DiagnosisCapabilities.CountAsync(x => x.OntologyTermId == id);

        public async Task<bool> ValidOntologyTerm(string id = null, string description = null, List<string> tags = null)
            => await QueryOntologyTerms(id, description, tags).AnyAsync();

        public async Task<bool> IsOntologyTermInUse(string id)
            => (await GetOntologyTermCollectionCapabilityCount(id) > 0);



        public Task<OntologyTerm> AddOntologyTermAsync(OntologyTerm diagnosis)
        {
            throw new System.NotImplementedException();
        }

        public Task AddOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteOntologyTermAsync(OntologyTerm diagnosis)
        {
            throw new System.NotImplementedException();
        }

        public Task<OntologyTerm> UpdateOntologyTermAsync(OntologyTerm diagnosis)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds)
        {
            throw new System.NotImplementedException();
        }

    }
}
