
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
        Task QueueCommittedData(int biobankId, bool replace);
    }
}
