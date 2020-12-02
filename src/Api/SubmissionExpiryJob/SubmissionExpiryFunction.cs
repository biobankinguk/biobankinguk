using System;
using System.Threading.Tasks;
using Biobanks.SubmissionExpiryJob.Services.Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Biobanks.SubmissionExpiryJob
{
    public class SubmissionExpiryFunction
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionExpiryFunction(ISubmissionService submissionService)
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
