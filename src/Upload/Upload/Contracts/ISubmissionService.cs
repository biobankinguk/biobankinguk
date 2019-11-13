using System.Collections.Generic;
using System.Threading.Tasks;
using Upload.Common.Data.Entities;
using Upload.Common.Types;

namespace Upload.Contracts
{
    /// <summary>
    /// Service for handling Submission metadata (e.g. status information)
    /// </summary>
    public interface ISubmissionService
    {
        /// <summary>
        /// Get a Submission's metadata record
        /// </summary>
        /// <param name="submissionId">The ID of the Submission to fetch</param>
        /// <returns>The requested Submission</returns>
        Task<Submission> Get(int submissionId);

        /// <summary>
        /// Get a list of metadata records for Submissions belonging to a given Organisation
        /// </summary>
        /// <param name="organisationId">The ID of the Organisation to list Submissions for</param>
        /// <param name="paging">Paging parameters</param>
        /// <returns>A paged list of Submissions for the requested Organisation</returns>
        Task<IEnumerable<Submission>> List(int organisationId, SubmissionPaginationParams paging);

        /// <summary>
        /// Get a list of metadata records for Submissions belonging to a given Organisation, which 
        /// are open and have not had all records processed. 
        /// </summary>
        /// <param name="organisationId">The ID of the Organisation to list Submissions for.</param>
        /// <returns>A list of Submissions for the requested Organisation.</returns>
        Task<IEnumerable<Submission>> ListSubmissionsInProgress(int organisationId);


        /// <summary>
        /// Creates a new submission with a status of 'Open'.
        /// </summary>
        /// <param name="totalRecords">Total number of records in the submission.</param>
        /// <param name="organisationId">The ID of the Organisation which the submission belongs to.</param>
        /// <returns></returns>
        Task<Submission> CreateSubmission(int totalRecords, int organisationId);

        /// <summary>
        /// Deletes a given submission from the data store.
        /// </summary>
        /// <param name="submissionId">ID of the submission to delete.</param>
        /// <returns></returns>
        Task DeleteSubmission(int submissionId);
    }
}
