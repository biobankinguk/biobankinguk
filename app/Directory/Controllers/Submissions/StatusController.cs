using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Submissions.Api.Auth;
using Core.Submissions.Models;
using Core.Submissions.Services.Contracts;
using Core.Submissions.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Submissions.Api.Controllers.Submissions
{
    /// <inheritdoc />
    /// <summary>
    /// This controller is for submission status actions only
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Submissions")]
    [Authorize(nameof(AuthPolicies.IsTokenAuthenticated))]
    public class StatusController : ControllerBase
    {
        private readonly ISubmissionService _submissions;

        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor, expects DI
        /// </summary>
        /// <param name="submissions"></param>
        /// <param name="mapper"></param>
        public StatusController(ISubmissionService submissions,
            IMapper mapper)
        {
            _submissions = submissions;
            _mapper = mapper;
        }

        /// <summary>
        /// List all submissions for the authenticated user's Biobank, paginated.
        /// </summary>
        /// <param name="paging">Pagination options, wrapped up from the query string.</param>
        /// <param name="biobankId">Biobank ID to operate on.</param>
        /// <returns>A paginated list of submission summaries.</returns>
        [HttpGet("biobank/{biobankId}")]
        [SwaggerResponse(200, Type = typeof(PaginatedSubmissionSummariesModel))]
        [SwaggerResponse(400, "Offset exceeds the total records available.")]
        [SwaggerResponse(400, "The parameters 'Since' and 'N' are mutually exclusive.")]
        public async Task<IActionResult> List(int biobankId, [FromQuery] SubmissionPaginationParams paging)
        {
            if (!User.HasClaim(CustomClaimTypes.BiobankId,
                biobankId.ToString()))
                return Forbid();

            if (paging.N > 0 && paging.Since != null)
                return BadRequest(
                    "the parameters 'since' and 'n' are mutually exclusive");

            // Try and fetch data
            var (total, submissions) = await _submissions.List(biobankId, paging);

            // Possible short circuits
            if (paging.Offset >= total && paging.Offset > 0)
                return BadRequest($"'offset' exceeds total of {total} records"); //Is this really bad request?

            if (total == 0)
                return Ok(new PaginatedSubmissionSummariesModel
                {
                    Submissions = new List<SubmissionSummaryModel>()
                });

            // Prepare the model for return
            var model = new PaginatedSubmissionSummariesModel
            {
                Submissions = _mapper.Map<ICollection<SubmissionSummaryModel>>(submissions)
            };

            // Set all the paging bits
            PaginateSubmissions(
                new Uri(HttpContext.Request.GetEncodedUrl()),
                biobankId,
                paging,
                model.Submissions.Count, total,
                ref model);

            return Ok(model);
        }

        /// <summary>
        /// Gets a status summary of a given submission.
        /// </summary>
        /// <param name="submissionId">The id of the submission.</param>
        /// <returns>A summary of the specified submission</returns>
        [HttpGet("{submissionId}")]
        [SwaggerResponse(200, Type = typeof(SubmissionSummaryModel))]
        [SwaggerResponse(404, "No submission found with the specified id.")]
        public async Task<IActionResult> Get(int submissionId)
        {
            var submission = await _submissions.Get(submissionId);

            if (submission == null)
                return NotFound();

            if (!User.HasClaim(CustomClaimTypes.BiobankId, submission.BiobankId.ToString()))
                return Forbid();

            return Ok(_mapper.Map<SubmissionSummaryModel>(submission));
        }

        #region Helpers

        private static void PaginateSubmissions<T>(
            Uri paginatedResourceUri,
            int biobankId,
            SubmissionPaginationParams paging,
            int count, int total, ref T model)
            where T : BasePaginatedModel
        {
            //prep next/previous
            var nextOffset = paging.Offset + count;

            var previousLimit = paging.Limit;
            var previousOffset = paging.Offset - previousLimit;
            if (previousOffset < 0)
            {
                previousLimit = paging.Offset;
                previousOffset = 0;
            }

            //update the model
            model.Count = count;
            model.Offset = paging.Offset;
            model.Total = total;
            model.Next = nextOffset < total
                ? GetPaginatedSubmissionsUri(
                    paginatedResourceUri,
                    biobankId,
                    new SubmissionPaginationParams
                    {
                        Offset = nextOffset,
                        Limit = paging.Limit,
                        Since = paging.Since,
                        N = paging.N
                    })
                : null;
            model.Previous = paging.Offset > 0
                ? GetPaginatedSubmissionsUri(
                    paginatedResourceUri,
                    biobankId,
                    new SubmissionPaginationParams
                    {
                        Offset = previousOffset,
                        Limit = previousLimit,
                        Since = paging.Since,
                        N = paging.N
                    })
                : null;
        }

        private static Uri GetPaginatedSubmissionsUri(
            Uri paginatedResourceUri, int biobankId, SubmissionPaginationParams paging)
            => new Uri(new UriBuilder(paginatedResourceUri)
            {
                Query = $"biobankId={biobankId}&offset={paging.Offset}&limit={paging.Limit}" +
                                (paging.Since != null ? $"&since={paging.Since}" : "") +
                                (paging.N > 0 ? $"&n={paging.N}" : "")
            }
                    .Uri
                    .PathAndQuery,
                UriKind.Relative);

        #endregion
    }
}
