using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
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

        public Task<OntologyTerm> AddOntologyTermAsync(OntologyTerm diagnosis)
        {
            throw new System.NotImplementedException();
        }

        public Task AddOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> CountOntologyTerms(string description = null, List<string> tags = null)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteOntologyTermAsync(OntologyTerm diagnosis)
        {
            throw new System.NotImplementedException();
        }

        public Task<OntologyTerm> GetOntologyTerm(string id = null, string description = null, List<string> tags = null, bool onlyDisplayable = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<int> GetOntologyTermCollectionCapabilityCount(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsOntologyTermInUse(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OntologyTerm>> ListOntologyTerms(string description = null, List<string> tags = null, bool onlyDisplayable = false)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OntologyTerm>> PaginateOntologyTerms(int start, int length, string description = null, List<string> tags = null)
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

        public Task<bool> ValidOntologyTerm(string id = null, string description = null, List<string> tags = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
