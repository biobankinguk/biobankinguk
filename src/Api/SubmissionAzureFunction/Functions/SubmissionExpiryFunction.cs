using System.Threading.Tasks;
using Biobanks.SubmissionAzureFunction.Services.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Biobanks.SubmissionAzureFunction
{
    public class SubmissionExpiryFunction
    {
        private readonly ISubmissionExpiryService _submissionService;

        public SubmissionExpiryFunction(ISubmissionExpiryService submissionService)
        {
            _submissionService = submissionService;
        }

        [FunctionName("SubmissionExpiryFunction")]
        public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo myTimer, ILogger log)
        {
            var organisationsToExpireSubmissionsFor = await _submissionService.GetOrganisationsWithExpiringSubmissions();

            foreach (var organisationId in organisationsToExpireSubmissionsFor)
            {
                await _submissionService.ExpireSubmissions(organisationId);
            }
        }
    }
}
