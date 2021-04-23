using Biobanks.Submissions.Api.Services.Contracts;
using Biobanks.Submissions.Core.Services.Contracts;
using Hangfire;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services
{
    /// <inheritdoc />
    public class HangfireQueueService : IBackgroundJobEnqueueingService
    {
        private readonly ICommitService _commitService;

        /// <inheritdoc />
        public HangfireQueueService(ICommitService commitService)
        {
            _commitService = commitService;
        }

        /// <inheritdoc />
        public async Task Commit(int biobankId, bool replace)
        {
            BackgroundJob.Enqueue(() => _commitService.CommitStagedData(replace, biobankId));
        }
    }
}
