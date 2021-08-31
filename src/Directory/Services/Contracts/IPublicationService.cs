using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IPublicationService
    {
        Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId);
        Task<IEnumerable<Publication>> GetOrganisationPublicationsAsync(Organisation organisation);
        Task<IEnumerable<Publication>> GetAcceptedOrganisationPublicationsAsync(Organisation organisation);

        Task<Publication> AddOrganisationPublicationAsync(Publication publication);
        Task<Publication> UpdateOrganisationPublicationAsync(Publication publication);
    }
}
