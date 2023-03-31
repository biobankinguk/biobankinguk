using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Aggregator.Services.Contracts
{
    public interface ICollectionAggregatorService
    {
        Task<Collection> GetCollection(int organisationId, string collectionName);

        Task UpdateCollection(Collection collection);
        
        Task AddCollection(Collection collection);

        Task DeleteCollection(int id);
    }
}
