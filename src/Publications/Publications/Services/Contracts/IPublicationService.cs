using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services
{
    public interface IPublicationService
    {
        Task<IEnumerable<PublicationDto>> GetOrganisationPublications(string organisationName);

        Task AddOrganisationPublications(string organisationName, IEnumerable<PublicationDto> publications);
    }
}
