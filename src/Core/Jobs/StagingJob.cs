using Core.Submissions.Models;
using Core.Submissions.Services.Contracts;
using System.Threading.Tasks;

namespace Core.Jobs
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
