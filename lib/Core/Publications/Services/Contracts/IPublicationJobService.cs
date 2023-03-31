using Biobanks.Publications.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Data.Entities;

namespace Biobanks.Publications.Services.Contracts
{
    public interface IPublicationJobService
    {
        Task<IEnumerable<Publication>> ListOrganisationPublications(int organisationId);

        Task AddOrganisationPublications(int organisationId, IEnumerable<PublicationDto> publications);
    }
}
