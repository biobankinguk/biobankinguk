using Biobanks.Entities.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory.Contracts
{
    public interface IPublicationService
    {
        /// <summary>
        /// Get all untracked Publication assoicated to a given Organisation
        /// </summary>
        /// <param name="organisationId">The organisation to which the publications belong. Filters the query by organisation Id.</param>
        /// <param name="acceptedOnly">Whether to return Publications that have been accepted by the Organisation, false by default</param>
        /// <returns>An Enumerable of all applicable untracked Publications</returns>
        Task<IEnumerable<Publication>> ListByOrganisation(int organisationId, bool acceptedOnly = false);

        /// <summary>
        /// Create a new Publication
        /// </summary>
        /// <returns>The newly created Publication with assigned Id</returns>
        Task<Publication> Create(Publication publication);

        /// <summary>
        /// Claim a Publication for an Organisation
        /// </summary>
        /// <param name="publicationId">The EPMRC Identifier of the Publication</param>
        /// <param name="organisationId">The Id of the Organisation</param>
        /// <param name="accept">If the Publication is to be accepted or rejected by the Organisation</param>
        /// <returns>Returns publication if found, or null if no such Publication is assigned to the given Organisation</returns>
        Task<Publication> Claim(string publicationId, int organisationId, bool accept = true);
    }
}
