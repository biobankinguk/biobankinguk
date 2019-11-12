using System.Threading.Tasks;

namespace Upload.Contracts
{
    /// <summary>
    /// Service for handling rejecting submissions.
    /// </summary>
    public interface IRejectService
    {
        /// <summary>
        /// Rejects the 'open' submissions for the organisation.
        /// </summary>
        /// <param name="organisationId">Unique identifier of the organisation on which to operate.</param>
        Task RejectStagedData(int organisationId);
    }
}