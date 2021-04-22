using Biobanks.Entities.Data;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface ICollectionService
    {
        Task DeleteCollectionAsync(Collection collection);
        
        Task UpdateCollectionAsync(Collection collection);
        
        Task AddCollectionAsync(Collection collection);
    }
}
