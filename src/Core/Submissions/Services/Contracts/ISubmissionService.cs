using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biobanks.Entities.Api;
using Core.Submissions.Models;
using Core.Submissions.Types;

namespace Core.Submissions.Services.Contracts
{
    /// <summary>
    /// Service for handling Submission metadata (e.g. status information)
    /// </summary>
    public interface ISubmissionService
    {
        /// <summary>
        /// Move the items identified in the queue item from the blob, validate and write to the staging tables
        /// </summary>
        /// <returns></returns>
        Task Staging(OperationsQueueItem operationQueueItem);
        /// <summary>
        /// Get a Submission's metadata record
        /// </summary>
        /// <param name="submissionId">The ID of the Submission to fetch</param>
        /// <returns>The requested Submission</returns>
        Task<Submission> Get(int submissionId);

        /// <summary>
        /// Get a list of metadata records for Submissions belonging to a given Biobank
        /// </summary>
        /// <param name="biobankId">The ID of the Biobank to list Submissions for</param>
        /// <param name="paging">Paging parameters</param>
        /// <returns>A paged list of Submissions for the requested Biobank</returns>
        Task<(int total, IEnumerable<Submission> submissions)>
            List(int biobankId, SubmissionPaginationParams paging);

        /// <summary>
        /// Get a list of metadata records for Submissions belonging to a given Biobank, which 
        /// are open and have not had all records processed. 
        /// </summary>
        /// <param name="biobankId">The ID of the Biobank to list Submissions for.</param>
        /// <returns>A list of Submissions for the requested Biobank.</returns>
        Task<IEnumerable<Submission>> ListSubmissionsInProgress(int biobankId);


        /// <summary>
        /// Creates a new submission with a status of 'Open'.
        /// </summary>
        /// <param name="totalRecords">Total number of records in the submission.</param>
        /// <param name="biobankId">The ID of the Biobank which the submission belongs to.</param>
        /// <returns></returns>
        Task<Submission> CreateSubmission(int totalRecords, int biobankId);

        /// <summary>
        /// Deletes a given submission from the data store.
        /// </summary>
        /// <param name="submissionId">ID of the submission to delete.</param>
        /// <returns></returns>
        Task DeleteSubmission(int submissionId);

        /// <summary>
        /// Marks n records as processed on the Submissions
        /// </summary>
        /// <param name="submissionId">ID of the submission to update the processed count for</param>
        /// <param name="n">The number of Records Processed</param>
        /// <returns></returns>
        Task ProcessRecords(int submissionId, int n);
    }
}

