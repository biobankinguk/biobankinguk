using Biobanks.Entities.Data;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IOrganisationService
    {
        Task<Organisation> GetById(int organisationId);
    }
}