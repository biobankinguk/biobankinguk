using System.Threading.Tasks;
using Biobanks.Submissions.Services.Contracts;

namespace Biobanks.Jobs
{
    public class ExpiryJob
    {
        private readonly ISubmissionExpiryService _submissionExpiryService;

        public ExpiryJob(ISubmissionExpiryService submissionExpiryService)
        {
            _submissionExpiryService = submissionExpiryService;
        }

        public async Task Run()
        {
            var organisationsToExpireSubmissionsFor = await _submissionExpiryService.GetOrganisationsWithExpiringSubmissions();

            foreach (var organisationId in organisationsToExpireSubmissionsFor)
            {
                await _submissionExpiryService.ExpireSubmissions(organisationId);
            }
        }
    }
}
