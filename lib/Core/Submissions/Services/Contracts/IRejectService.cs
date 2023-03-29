using System.Threading.Tasks;

namespace Core.Submissions.Services.Contracts
{
    /// <summary>
    /// Service for handling rejecting submissions.
    /// </summary>
    public interface IRejectService
    {
        /// <summary>
        /// Rejects the 'open' submissions for the biobank.
        /// </summary>
        /// <param name="organisationId">Unique identifier of the organisation (biobank) on which to operate.</param>
        Task RejectStagedData(int organisationId);
    }
}