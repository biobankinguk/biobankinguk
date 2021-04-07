using System.Threading.Tasks;

using AzFunctions.Types;

using Biobanks.Submissions.Core.Services.Contracts;
using Microsoft.Azure.Functions.Worker;

namespace Biobanks.Submissions.AzFunctions
{
    public class SubmissionExpiryFunction
    {
        private readonly ISubmissionExpiryService _submissions;

        public SubmissionExpiryFunction(ISubmissionExpiryService submissionService)
        {
            _submissions = submissionService;
        }

        [Function("Submissions_Expiry")]
        public async Task Run([TimerTrigger("0 3 * * *")] TimerInfo timer)
        {
            var organisationsToExpireSubmissionsFor = await _submissions.GetOrganisationsWithExpiringSubmissions();

            foreach (var organisationId in organisationsToExpireSubmissionsFor)
            {
                await _submissions.ExpireSubmissions(organisationId);
            }
        }
    }
}
