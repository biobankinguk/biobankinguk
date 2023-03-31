using System.Threading.Tasks;

namespace Biobanks.Submissions.Services.Contracts
{
    /// <summary>
    /// Service for handling committing submissions.
    /// </summary>
    public interface ICommitService
    {
        /// <summary>
        /// Commits the 'open' submissions for the biobank.
        /// </summary>
        /// <param name="replace">True to replace all existing data for the biobank, or false to merge the two.</param>
        /// /// <param name="organisationId">Unique identifier of the organisation (biobank) on which to operate.</param>
        Task CommitStagedData(bool replace, int organisationId);
    }
}
