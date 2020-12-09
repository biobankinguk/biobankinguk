using System.Threading.Tasks;
using Biobanks.ExpiryJob.Services.Contracts;
using Microsoft.Azure.WebJobs;

namespace Biobanks.ExpiryJob
{
    public class Functions
    {
        private readonly ISubmissionService _submissionService;

        public Functions(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public async Task ProcessQueueMessage([TimerTrigger("0 3 * * *")]TimerInfo timerInfo)
        {
            var organisationsToExpireSubmissionsFor = await _submissionService.GetOrganisationsWithExpiringSubmissions();

            foreach (var organisationId in organisationsToExpireSubmissionsFor)
            {
                await _submissionService.ExpireSubmissions(organisationId);
            }
        }
    }
}
