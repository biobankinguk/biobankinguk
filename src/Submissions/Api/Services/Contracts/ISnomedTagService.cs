using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Contracts
{
    /// <summary>
    /// Service for handling Snomed Tag reference data.
    /// </summary>
    public interface ISnomedTagService
    {
        /// <summary>
        /// Get a Snomed Tag entity.
        /// </summary>
        /// <param name="snomedTagId">The ID of the Snomed Tag to fetch.</param>
        /// <returns>The requested Snomed Tag.</returns>
        Task<SnomedTag> Get(int snomedTagId);

        /// <summary>
        /// Get a list of Snomed Tag entities.
        /// </summary>
        /// <returns>A list of Snomed Tag entities.</returns>
        Task<IEnumerable<SnomedTag>> List();

        /// <summary>
        /// Inserts a new Snomed Tag entity into the repository.
        /// </summary>
        /// <param name="snomedTag">The Snomed Tag object to insert.</param>
        /// <returns>The created Snomed Tag.</returns>
        Task<SnomedTag> Create(SnomedTag snomedTag);
        
        /// <summary>
        /// Updates an existing Snomed Tag entity in the respository.
        /// </summary>
        /// <param name="snomedTag">The Snomed Tag object to update.</param>
        /// <returns></returns>
        Task Update(SnomedTag snomedTag);

        /// <summary>
        /// Get a Snomed Tag entity.
        /// </summary>
        /// <param name="value">The value of the Snomed Tag to fetch.</param>
        /// <returns>The requested Snomed Tag.</returns>
        Task<SnomedTag> GetByValue(string value);
    }
}