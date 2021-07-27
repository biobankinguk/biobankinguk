using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface ICollectionService
    {
        Task<bool> DeleteCollection(int id);

        Task<Collection> AddCollection(Collection collection);
        Task<Collection> UpdateCollection(Collection collection);

        Task<Collection> GetCollection(int id);
        Task<Collection> GetEntireCollection(int id);
        Task<Collection> GetIndexableCollection(int id);

        Task<IEnumerable<Collection>> ListCollections(int organisationId = default);
        Task<IEnumerable<Collection>> ListCollectionsByOntologyTerm(string ontologyTerm);

        Task<bool> IsFromApi(int id);
        Task<bool> HasSampleSets(int id);

        //Task<Collection> GetCollectionWithSampleSetsById(int id);
    }
}
