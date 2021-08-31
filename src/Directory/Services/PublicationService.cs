using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class PublicationService : IPublicationService
    {
        public Task<Publication> AddOrganisationPublicationAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Publication>> GetAcceptedOrganisationPublicationsAsync(Organisation organisation)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Publication>> GetOrganisationPublicationsAsync(Organisation organisation)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Publication>> ListOrganisationPublications(int biobankId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Publication> UpdateOrganisationPublicationAsync(Publication publication)
        {
            throw new System.NotImplementedException();
        }
    }
}
