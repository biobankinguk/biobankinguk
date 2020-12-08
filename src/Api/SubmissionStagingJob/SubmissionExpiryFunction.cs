using System;
using System.Threading.Tasks;
using Biobanks.SubmissionStagingJob.Services.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Biobanks.SubmissionStagingJob
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
