using System;
using System.Threading.Tasks;
using Biobanks.Directory.Services.Submissions.Contracts;
using Biobanks.Jobs;
using Biobanks.Submissions.Models;
using Biobanks.Submissions.Types;
using Hangfire;

namespace Biobanks.Directory.Services.Submissions
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
