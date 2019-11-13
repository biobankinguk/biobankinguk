﻿using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using Upload.Contracts;

namespace Upload.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// This controller serves as a sub-controller for committing submissions.
    /// </summary>
    [Route("{organisationId}/[controller]")]
    [ApiController]
    public class CommitController : ControllerBase
    {
        private readonly ICommitService _commitService;
        private readonly ISubmissionService _submissionService;

        /// <inheritdoc />
        public CommitController(ICommitService commitService,
            ISubmissionService submissionService)
        {
            _commitService = commitService;
            _submissionService = submissionService;
        }

        /// <summary>
        /// Commits all staged data changes to the live database.
        /// Optionally replaces all live data with the staged versions, based on type.
        /// </summary>
        /// <param name="type">
        /// replace = all live data is deleted and replaced with the staged data.
        /// update = staged changes (deletions, inserts, udpates) are applied to the current live data.
        /// </param>
        /// <param name="organisationId">Organisation ID to operate on.</param>
        [HttpPost]
        [SwaggerResponse(204, Description = "The commit completed successfully.")]
        [SwaggerResponse(400, Description = "Expected Type parameter to specify 'update' or 'replace'.")]
        [SwaggerResponse(400, Description = "Organisation ID claim in bad format.")]
        [SwaggerResponse(400, Description = "There are Open Submissions which have not been processed.")]
        public async Task<IActionResult> Post(int organisationId, string type)
        {
            if (type == null)
                return BadRequest("Expected Type parameter to specify 'update' or 'replace'");

            //if (!User.HasClaim(CustomClaimTypes.OrganisationId,
           //     organisationId.ToString()))
            //    return Forbid();

            var submissionsInProgress = await _submissionService.ListSubmissionsInProgress(organisationId);
            if (submissionsInProgress.Any())
            {
                return BadRequest(JsonConvert.SerializeObject(submissionsInProgress));
            }

            //Hangfire stuff 
            BackgroundJob.Enqueue(() => _commitService.CommitStagedData(type.Equals("replace", StringComparison.OrdinalIgnoreCase), organisationId));

            return NoContent();
        }
    }
}
