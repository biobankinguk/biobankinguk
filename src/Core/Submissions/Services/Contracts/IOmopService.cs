using Biobanks.Omop.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Submissions.Services.Contracts
{
    /// <summary>
    /// Service for accessing Omop
    /// </summary>
    public interface IOmopService
    {
        /// <summary>
        /// Returns a list of Organisation IDs which have expiring submissions. 
        /// </summary>
        /// <returns>The Organisation IDs with expiring submissions.</returns>
        Task<IEnumerable<ConditionOccurence>> ListSynonyms(int snomedCode);

    }
}