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
        /// 
        /// </summary>
        /// <returns></returns>
        Task Staging(OperationsQueueItem operationQueueItem);
    }
}
