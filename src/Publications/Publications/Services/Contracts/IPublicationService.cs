using Biobanks.Publications.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Publications.Services
{
    public interface IPublicationService
    {
        Task AddOrganisationPublications(int organisationId, IEnumerable<PublicationDto> publications);
    }
}
