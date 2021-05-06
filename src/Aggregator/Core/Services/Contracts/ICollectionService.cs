using Biobanks.Entities.Data;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface ICollectionService
    {
        Task<Collection> GetCollectionAsync(int organisationId, string collectionName);

        Task UpdateCollectionAsync(Collection collection);
        
        Task AddCollectionAsync(Collection collection);

        Task DeleteCollectionAsync(Collection collection);

        Task DeleteSampleSetById(int id);

        Task DeleteMaterialDetailsBySampleSetId(int id);
    }
}
