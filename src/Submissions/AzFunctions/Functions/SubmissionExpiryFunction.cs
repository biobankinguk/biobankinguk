using System.Threading.Tasks;

using AzFunctions.Types;

using Biobanks.Submissions.Core.Services.Contracts;
using Microsoft.Azure.Functions.Worker;

namespace Biobanks.Submissions.AzFunctions
{
    public class SubmissionExpiryFunction
    {
        private readonly ISubmissionExpiryService _submissionService;

        public SubmissionExpiryFunction(ISubmissionExpiryService submissionService)
        {
            _submissionService = submissionService;
        }

        [Function("Submissions_Expiry")]
        public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo _)
        {
            var organisationsToExpireSubmissionsFor = await _submissionService.GetOrganisationsWithExpiringSubmissions();

            foreach (var organisationId in organisationsToExpireSubmissionsFor)
            {
                await _submissionService.ExpireSubmissions(organisationId);
            }
        }
    }
}
