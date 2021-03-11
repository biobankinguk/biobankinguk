using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Biobanks.Submissions.Core.Exceptions;
using Biobanks.Submissions.Core.Types;

namespace Biobanks.Submissions.Core.Services.Contracts
{
    /// <summary>
    /// Service for handling Submission Error information
    /// </summary>
    public interface IErrorService
    {
        /// <summary>
        /// List Errors for a given Submission ID
        /// </summary>
        /// <param name="submissionId">The ID of the Submission to list Errors for</param>
        /// <param name="paging">Pagination parameters</param>
        /// <returns>A list of Errors for the requested Submission</returns>
        Task<(int biobankId, int total, IEnumerable<Error> errors)> List(
            int submissionId, PaginationParams paging);

        /// <summary>
        /// Get a single Submission Error by its ID
        /// </summary>
        /// <param name="errorId">The ID of the Error to fetch</param>
        /// <returns>The requested Error</returns>
        Task<Error> Get(int errorId);

        Task Add(int submissionId, Operation op, string type, ICollection<BiobanksValidationResult> messages, int biobankId);
    }
}
