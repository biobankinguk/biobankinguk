using Core.Submissions.Services.Contracts;
using System.Threading.Tasks;

namespace Core.Jobs
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
