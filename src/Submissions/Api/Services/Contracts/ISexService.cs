using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Shared.ReferenceData;

namespace Biobanks.Submissions.Api.Services.Contracts
{
    /// <summary>
    /// Service for handling Sex reference data.
    /// </summary>
    public interface ISexService
    {
        /// <summary>
        /// Get a Sex entity.
        /// </summary>
        /// <param name="sexId">The ID of the Sex to fetch.</param>
        /// <returns>The requested Sex.</returns>
        Task<Sex> Get(int sexId);

        /// <summary>
        /// Get a list of Sex entities.
        /// </summary>
        /// <returns>A list of Sex entities.</returns>
        Task<IEnumerable<Sex>> List();

        /// <summary>
        /// Inserts a new Sex entity into the repository.
        /// </summary>
        /// <param name="sex">The Sex object to insert.</param>
        /// <returns>The created Sex.</returns>
        Task<Sex> Create(Sex sex);
        
        /// <summary>
        /// Updates an existing Sex entity in the respository.
        /// </summary>
        /// <param name="sex">The Sex object to update.</param>
        /// <returns></returns>
        Task Update(Sex sex);

        /// <summary>
        /// Get a Sex entity.
        /// </summary>
        /// <param name="value">The value of the Sex to fetch.</param>
        /// <returns>The requested Sex.</returns>
        Task<Sex> GetByValue(string value);

    }
}
