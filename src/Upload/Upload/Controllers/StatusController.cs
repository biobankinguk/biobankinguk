using AutoMapper;
using Upload.Common.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upload.Common.Types;
using Upload.Contracts;
using Upload.Common.DTO;
using Upload.DTOs;

namespace Upload.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// This controller is for submission status actions only
    /// </summary>
    [Route("[controller]")]
    [ApiController]
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
        /// List all submissions for the authenticated user's Organisation, paginated.
        /// </summary>
        /// <param name="paging">Pagination options, wrapped up from the query string.</param>
        /// <param name="organisationId">Organisation ID to operate on.</param>
        /// <returns>A paginated list of submission summaries.</returns>
        [HttpGet("organisation/{organisationId}")]
        [SwaggerResponse(200, Type = typeof(PaginatedSubmissionSummariesDto))]
        [SwaggerResponse(403, "Access to the requested submission denied.")]
        [SwaggerResponse(400, "Offset exceeds the total records available.")]
        [SwaggerResponse(400, "The parameters 'Since' and 'N' are mutually exclusive.")]
        public async Task<IActionResult> List(int organisationId, [FromQuery]SubmissionPaginationParams paging)
        {
            //  if (!User.HasClaim(CustomClaimTypes.OrganisationId,
            //      organisationId.ToString()))
            //      return Forbid();

            if (paging.N > 0 && paging.Since != null)
                return BadRequest(
                    "the parameters 'since' and 'n' are mutually exclusive");

            // Try and fetch data
            var submissions = await _submissions.List(organisationId, paging);
            var submissionCount = submissions.Count();

            // Possible short circuits
            if (paging.Offset >= submissionCount && paging.Offset > 0)
                return BadRequest($"'offset' exceeds total of {submissionCount} records"); //Is this really bad request?

            if (submissionCount == 0)
                return Ok(new PaginatedSubmissionSummariesDto
                {
                    Submissions = new List<SubmissionSummaryDto>()
                });

            // Prepare the model for return
            var model = new PaginatedSubmissionSummariesDto
            {
                Submissions = _mapper.Map<List<SubmissionSummaryDto>>(submissions)
            };

            // Set all the paging bits
            PaginateSubmissions(
                new Uri(HttpContext.Request.GetEncodedUrl()),
                organisationId,
                paging,
                model.Submissions.Count, submissionCount,
                ref model);

            return Ok(model);
        }

        /// <summary>
        /// Gets a status summary of a given submission.
        /// </summary>
        /// <param name="submissionId">The id of the submission.</param>
        /// <returns>A summary of the specified submission</returns>
        [HttpGet("{submissionId}")]
        [SwaggerResponse(200, Type = typeof(SubmissionSummaryDto))]
        [SwaggerResponse(403, "Access to the requested submission denied.")]
        [SwaggerResponse(404, "No submission found with the specified id.")]
        public async Task<IActionResult> Get(int submissionId)
        {
            var submission = await _submissions.Get(submissionId);
            if (submission == null) return NotFound();

            // if (!User.HasClaim(CustomClaimTypes.OrganisationId,
            //     submission.OrganisationId.ToString()))
            //     return Forbid();

            return Ok(_mapper.Map<SubmissionSummaryDto>(submission));
        }

        #region Helpers

        private static void PaginateSubmissions<T>(
            Uri paginatedResourceUri,
            int organisationId,
            SubmissionPaginationParams paging,
            int count, int total, ref T model)
            where T : BasePaginatedDto
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
                    organisationId,
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
                    organisationId,
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
            Uri paginatedResourceUri, int organisationId, SubmissionPaginationParams paging)
            => new Uri(new UriBuilder(paginatedResourceUri)
            {
                Query = $"organisationId={organisationId}&offset={paging.Offset}&limit={paging.Limit}" +
                                (paging.Since != null ? $"&since={paging.Since}" : "") +
                                (paging.N > 0 ? $"&n={paging.N}" : "")
            }
                    .Uri
                    .PathAndQuery,
                UriKind.Relative);

        #endregion
    }
}
