using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IEMPCService
    {
        Task<PublicationDTO> GetPublicationById(int publicationId);

        Task<List<PublicationDTO>> GetOrganisationPublications(string biobank);
    }
}
