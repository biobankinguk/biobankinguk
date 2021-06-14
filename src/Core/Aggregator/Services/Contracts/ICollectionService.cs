using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Services.Contracts
{
    public interface ICollectionService
    {
        Task<Collection> GetCollection(int organisationId, string collectionName);

        Task UpdateCollection(Collection collection);
        
        Task AddCollection(Collection collection);

        Task DeleteCollection(int id);

        Task DeleteSampleSetByIds(IEnumerable<int> ids);

        Task DeleteMaterialDetailsBySampleSetIds(IEnumerable<int> ids);

    }
}
