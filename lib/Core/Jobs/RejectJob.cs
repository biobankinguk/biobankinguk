using Core.Submissions.Services.Contracts;
using System.Threading.Tasks;

namespace Core.Jobs
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
