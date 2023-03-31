using System.Threading.Tasks;
using Biobanks.Submissions.Models;
using Biobanks.Submissions.Services.Contracts;

namespace Biobanks.Jobs
{
    public class StagingJob
    {
        public ISubmissionService _submissionService;

        public StagingJob(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task Run(OperationsQueueItem item)
        {
            await _submissionService.Staging(item);
        }
    }
}
