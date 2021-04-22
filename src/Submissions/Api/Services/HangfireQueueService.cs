using Biobanks.Submissions.Api.Services.Contracts;
using Biobanks.Submissions.Core.Services.Contracts;
using Hangfire;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services
{
    public class HangfireQueueService : IBackgroundJobEnqueueingService
    {
        private readonly ICommitService _commitService;

        public HangfireQueueService(ICommitService commitService)
        {
            _commitService = commitService;
        }

        public async Task QueueCommittedData(int biobankId, bool replace)
        {
            BackgroundJob.Enqueue(() => _commitService.CommitStagedData(replace, biobankId));
        }
    }
}
