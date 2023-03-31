using System.Threading.Tasks;
using Biobanks.Submissions.Services.Contracts;

namespace Biobanks.Jobs
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
