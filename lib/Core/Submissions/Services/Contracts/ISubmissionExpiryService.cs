using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Services.Contracts
{
    public interface ISubmissionExpiryService
    {
        /// <summary>
        /// Returns a list of Organisation IDs which have expiring submissions. 
        /// </summary>
        /// <returns>The Organisation IDs with expiring submissions.</returns>
        Task<IEnumerable<int>> GetOrganisationsWithExpiringSubmissions();

        /// <summary>
        /// Rejects the submissions for a given Organisation.
        /// </summary>
        /// <param name="organisationId">The ID of the Organisation to Reject submissions for.</param>
        /// <returns></returns>
        Task ExpireSubmissions(int organisationId);
    }
}
