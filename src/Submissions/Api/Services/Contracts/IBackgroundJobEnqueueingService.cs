
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Contracts
{
    /// <summary>
    /// Wrapper service for handling
    /// </summary>
    public interface IBackgroundJobEnqueueingService
    {
        /// <summary>
        /// Confirms that the given Biobank is ready to commit its data to the live tables
        /// </summary>
        /// <param name="biobankId">Identifier for the Biobank</param>
        /// <param name="replace">Confirms if all the current data in the live table should be deleted prior this commit</param>
        /// <returns></returns>
        Task Commit(int biobankId, bool replace);
        /// <summary>
        /// Rejects, and therefore deletes all of the currently staged data for a given Biobank
        /// </summary>
        /// <param name="biobankId">Identifier for the Biobank</param>
        /// <returns></returns>
        Task Reject(int biobankId);
    }
}
