using Core.Submissions.Services.Contracts;
using System.Threading.Tasks;

namespace Core.Jobs
{
    public class CommitJob
    {
        private readonly ICommitService _commitService;

        public CommitJob(ICommitService commitService)
        {
            _commitService = commitService;
        }

        public async Task Run(int biobankId, bool replace)
        {
            await _commitService.CommitStagedData(replace, biobankId);
        }
    }
}
