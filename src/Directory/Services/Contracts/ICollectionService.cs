using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface ICollectionService
    {
        Task<bool> IsCollectionFromApi(int id);

        Task<Collection> GetCollection(int id);
        Task<Collection> GetIndexableCollection(int id);

        Task<IEnumerable<Collection>> ListCollections(int organisationId = default);
        Task<IEnumerable<Collection>> ListCollectionsByOntologyTerm(string ontologyTerm);

        //Task<Collection> GetCollectionWithSampleSetsById(int id);
    }
}
