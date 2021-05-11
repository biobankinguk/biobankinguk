using Biobanks.Entities.Data;
using Biobanks.Publications.Core.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Core.Services.Contracts
{
    public interface IPublicationService
    {
        Task<IEnumerable<Publication>> ListOrganisationPublications(int organisationId);

        Task AddOrganisationPublications(int organisationId, IEnumerable<PublicationDto> publications);
    }
}
