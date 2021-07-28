using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IOntologyTermService
    {
        Task<IEnumerable<OntologyTerm>> ListOntologyTerms(string description = null, List<string> tags = null, bool onlyDisplayable = false);
        Task<IEnumerable<OntologyTerm>> PaginateOntologyTerms(int start, int length, string description = null, List<string> tags = null);
        
        Task<OntologyTerm> GetOntologyTerm(string id = null, string description = null, List<string> tags = null, bool onlyDisplayable = false);
        
        Task<bool> ValidOntologyTerm(string id = null, string description = null, List<string> tags = null);
        Task<bool> IsOntologyTermInUse(string id);
        
        Task<int> CountOntologyTerms(string description = null, List<string> tags = null);
        Task<int> GetOntologyTermCollectionCapabilityCount(string id);


        Task DeleteOntologyTermAsync(OntologyTerm diagnosis);
        Task<OntologyTerm> UpdateOntologyTermAsync(OntologyTerm diagnosis);
        Task<OntologyTerm> AddOntologyTermAsync(OntologyTerm diagnosis);
        Task AddOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds);
        Task UpdateOntologyTermWithMaterialTypesAsync(OntologyTerm ontologyTerm, List<int> materialTypeIds);
    }
}
