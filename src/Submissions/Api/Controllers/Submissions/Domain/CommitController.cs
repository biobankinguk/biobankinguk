using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;

using Microsoft.AspNetCore.Authorization;
using Core.Submissions.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Biobanks.Submissions.Api.Services.Submissions.Contracts;

namespace Biobanks.Submissions.Api.Controllers.Submissions.Domain
{
    /// <inheritdoc />
    /// <summary>
    /// This controller serves as a sub-controller for committing submissions.
    /// </summary>
    [Route("{biobankId}/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Submissions")]
    [Authorize(nameof(AuthPolicies.IsTokenAuthenticated))]
    public class CommitController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;
        private readonly IBackgroundJobEnqueueingService _backgroundJobEnqueueingService;

        /// <inheritdoc />
        public CommitController(
            ISubmissionService submissionService, IBackgroundJobEnqueueingService backgroundJobEnqueueingService)
        {
            _submissionService = submissionService;
            _backgroundJobEnqueueingService = backgroundJobEnqueueingService;
        }

        /// <summary>
        /// Commits all staged data changes to the live database.
        /// Optionally replaces all live data with the staged versions, based on type.
        /// </summary>
        /// <param name="type">
        /// replace = all live data is deleted and replaced with the staged data.
        /// update = staged changes (deletions, inserts, udpates) are applied to the current live data.
        /// </param>
        /// <param name="biobankId">Biobank ID to operate on.</param>
        [HttpPost]
        [SwaggerResponse(204, Description = "The commit completed successfully.")]
        [SwaggerResponse(400, Description = "Expected Type parameter to specify 'update' or 'replace'.")]
        [SwaggerResponse(400, Description = "Organisation ID claim in bad format.")]
        [SwaggerResponse(400, Description = "There are Open Submissions which have not been processed.")]
        public async Task<IActionResult> Post(int biobankId, string type = null)
        {
            if (type == null)
                return BadRequest("Expected Type parameter to specify 'update' or 'replace'");

            if (!User.HasClaim(CustomClaimTypes.BiobankId,
                biobankId.ToString()))
                return Forbid();

            var submissionsInProgress = await _submissionService.ListSubmissionsInProgress(biobankId);
            if (submissionsInProgress.Any())
            {
                return BadRequest(submissionsInProgress);
            }

            //BackgroundJobEnqueueingService will then call either _queueWriteService or Hangfire to 
            //queue the job up
            //TODO: later PBI which will sort out conditional DI for which service to implement
            await _backgroundJobEnqueueingService.Commit(biobankId, type.Equals("replace", StringComparison.OrdinalIgnoreCase));

            return Accepted();
        }
    }
}
