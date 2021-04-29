using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IOrganisationService
    {
        Task<AccessCondition> GetAccessCondition(Organisation organisation);

        Task<CollectionType> GetCollectionType(Organisation organisation);
    }
}
