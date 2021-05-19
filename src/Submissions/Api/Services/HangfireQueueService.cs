using Biobanks.Submissions.Api.Services.Contracts;
using Core.Jobs;
using Hangfire;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class HangfireQueueService : IBackgroundJobEnqueueingService
    {
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
