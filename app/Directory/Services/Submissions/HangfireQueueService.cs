using Biobanks.Submissions.Api.Services.Submissions.Contracts;
using Core.Jobs;
using Core.Submissions.Models;
using Core.Submissions.Types;
using Hangfire;
using System;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Submissions
{
    /// <inheritdoc />
    public class HangfireQueueService : IBackgroundJobEnqueueingService
    {
        /// <inheritdoc />
        public Task Stage(int biobankId, int submissionId, Guid blobId, string blobType, Operation op)
        {
            BackgroundJob.Enqueue<StagingJob>(x => x.Run(
                new OperationsQueueItem
                {
                    SubmissionId = submissionId,
                    Operation = op,
                    BlobId = blobId,
                    BlobType = blobType,
                    BiobankId = biobankId
                }
            ));

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Commit(int biobankId, bool replace)
        {
            BackgroundJob.Enqueue<CommitJob>(x => x.Run(biobankId, replace));
            return Task.CompletedTask;
        }
        
        /// <inheritdoc />
        public Task Reject(int biobankId)
        {
            BackgroundJob.Enqueue<RejectJob>(x => x.Run(biobankId));
            return Task.CompletedTask;
        }
  }
}
