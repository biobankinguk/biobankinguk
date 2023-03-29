using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Core.Submissions.Exceptions;
using Core.Submissions.Types;

namespace Core.Submissions.Services.Contracts
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

        /// <summary>
        /// Add Errors based on Validation Exceptions
        /// </summary>
        /// <param name="submissionId">ID of the Submission to add Errors to</param>
        /// <param name="op">The Operation in question</param>
        /// <param name="type">Type (?)</param>
        /// <param name="messages">The Validation exceptions</param>
        /// <param name="biobankId">ID of the Biobank this submission is for</param>
        /// <returns></returns>
        Task Add(int submissionId, Operation op, string type, ICollection<BiobanksValidationResult> messages, int biobankId);
    }
}
