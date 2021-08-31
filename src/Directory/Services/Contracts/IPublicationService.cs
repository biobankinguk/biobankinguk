using Biobanks.Entities.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services.Contracts
{
    public interface IPublicationService
    {
        Task<IEnumerable<Publication>> ListByOrganisation(int organisationId, bool acceptedOnly = false);

        Task<Publication> Create(Publication publication);

        Task<Publication> Update(string publicationId, int organisationId, Action<Publication> updates);
    }
}
