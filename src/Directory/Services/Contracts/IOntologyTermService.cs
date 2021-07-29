using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IOntologyTermService
    {

        Task<IEnumerable<OntologyTerm>> List(string description = null, List<string> tags = null, bool onlyDisplayable = false);

        Task<IEnumerable<OntologyTerm>> ListPaginated(int skip, int take, string description = null, List<string> tags = null, bool onlyDisplayable = false);

        Task Delete(string id);

        Task<OntologyTerm> Get(string id = null, string description = null, List<string> tags = null, bool onlyDisplayable = false);

        Task<OntologyTerm> Update(OntologyTerm ontologyTerm);

        Task<OntologyTerm> Create(OntologyTerm ontologyTerm);

        Task<bool> IsValid(string id = null, string description = null, List<string> tags = null);
        Task<bool> IsInUse(string id);
        
        Task<int> Count(string description = null, List<string> tags = null);
        
        Task<int> CountCollectionCapabilityUsage(string ontologyTermId);
    }
}
