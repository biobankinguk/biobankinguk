using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services
{
    public interface IPublicationService
    {
        Task<IEnumerable<Publication>> GetOrganisationPublications(string organisationName);

        Task AddOrganisationPublications(string organisationName, IEnumerable<Publication> publications);
    }
}
