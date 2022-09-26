using Biobanks.Publications.Dto;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Services.Contracts
{
    public interface IPublicationJobService
    {
        Task<IEnumerable<Publication>> ListOrganisationPublications(int organisationId);

        Task AddOrganisationPublications(int organisationId, IEnumerable<PublicationDto> publications);
    }
}
