using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Common.Data.Entities.ReferenceData;

namespace Biobanks.SubmissionApi.Services.Contracts
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

    }
}
