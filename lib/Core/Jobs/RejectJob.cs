using System.Threading.Tasks;
using Biobanks.Submissions.Services.Contracts;

namespace Biobanks.Jobs
{
    public class RejectJob
    {
        private readonly IRejectService _rejectService;
        public RejectJob(IRejectService rejectService)
        {
            _rejectService = rejectService;
        }

        public async Task Run(int biobankId)
        {
            await _rejectService.RejectStagedData(biobankId);
        }
    }
}
