using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Core.Submissions.Types;
using Core.Submissions.Services.Contracts;

namespace Biobanks.Submissions.Api.Controllers.Submissions
{
    /// <summary>
    /// This controller serves as a sub-controller for submission errors only
    /// </summary>
    [Route("status/{submissionId}/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Submissions")]
    [Authorize(nameof(AuthPolicies.IsTokenAuthenticated))]
    public class ErrorController : ControllerBase
    {
        private readonly IErrorService _errors;
        private readonly IMapper _mapper;

        /// <summary>
        /// Controller, expects DI
        /// </summary>
        public ErrorController(
            IErrorService errors, IMapper mapper)
        {
            _errors = errors;
            _mapper = mapper;
        }

        /// <summary>
        /// List all errors for a given submission, paginated.
        /// </summary>
        /// <param name="submissionId">The id of the submission.</param>
        /// <param name="paging">Pagination options, wrapped up from the query string.</param>
        /// <returns>A paginated list of errors.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(PaginatedErrorsModel))]
        [SwaggerResponse(404, "No submission found with the specified id.")]
        [SwaggerResponse(400, "Offset exceeds the total records available.")]
        public async Task<IActionResult> ListErrors(int submissionId, [FromQuery] PaginationParams paging)
        {
            var (subBiobankId, total, errors) = await _errors.List(submissionId, paging);

            // a few short circuit scenarios
            if (paging.Offset >= total && paging.Offset > 0)
                return BadRequest($"'offset' exceeds total of {total} records"); //Is this really bad request?

            if (!User.HasClaim(CustomClaimTypes.BiobankId,
                subBiobankId.ToString()))
                return Forbid();

            if (total == 0)
                return Ok(new PaginatedErrorsModel()
                {
                    Errors = new List<ErrorModel>()
                });

            // Prepare the return model
            var model = new PaginatedErrorsModel
            {
                Errors = _mapper.Map<ICollection<ErrorModel>>(errors)
            };

            // Set all the paging bits
            Utils.Paginate(
                new Uri(HttpContext.Request.GetEncodedUrl()),
                paging,
                model.Errors.Count, total,
                ref model);

            return Ok(model);
        }

        /// <summary>
        /// Get a single error for a submission by id.
        /// </summary>
        /// <param name="submissionId">The id of the submission.</param>
        /// <param name="errorId">The id of the error.</param>
        /// <returns>A single error.</returns>
        [HttpGet("{errorId}")]
        [SwaggerResponse(200, Type = typeof(ErrorModel))]
        [SwaggerResponse(404, "No error found with the specified id matching the specified submission.")]
        public async Task<IActionResult> GetError(int submissionId, int errorId)
        {
            var error = await _errors.Get(errorId);

            if (error?.Submission.Id != submissionId) return NotFound();

            if (!User.HasClaim(CustomClaimTypes.BiobankId,
                error.Submission.BiobankId.ToString()))
                return Forbid();

            return Ok(_mapper.Map<ErrorModel>(error));
        }
    }
}
