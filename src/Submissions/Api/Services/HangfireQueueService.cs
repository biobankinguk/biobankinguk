using Biobanks.Submissions.Api.Services.Contracts;
using Core.Submissions.Services.Contracts;
using Hangfire;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class HangfireQueueService : IBackgroundJobEnqueueingService
    {
        private readonly ICommitService _commitService;
        private readonly IRejectService _rejectService;

        /// <inheritdoc />
        public HangfireQueueService(ICommitService commitService, IRejectService rejectService)
        {
            _commitService = commitService;
            _rejectService = rejectService;
        }

        /// <inheritdoc />
        public Task Commit(int biobankId, bool replace)
        {
            BackgroundJob.Enqueue(() => _commitService.CommitStagedData(replace, biobankId));
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task Reject(int biobankId)
        {
            BackgroundJob.Enqueue(() => _rejectService.RejectStagedData(biobankId));
            return Task.CompletedTask;
        }
    }
}
