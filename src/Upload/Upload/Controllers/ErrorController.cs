using AutoMapper;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Upload.Common;
using Upload.Common.Types;
using Upload.Contracts;
using Upload.DTO;

namespace Upload.Controllers
{
    /// <summary>
    /// This controller serves as a sub-controller for submission errors only
    /// </summary>
    [Route("status/{submissionId}/[controller]")]
    [ApiController]
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
        [SwaggerResponse(200, Type = typeof(PaginatedErrorsDto))]
        [SwaggerResponse(403, "Access to the requested submission denied.")]
        [SwaggerResponse(404, "No submission found with the specified id.")]
        [SwaggerResponse(400, "Offset exceeds the total records available.")]
        public async Task<IActionResult> ListErrors(int submissionId, [FromQuery]PaginationParams paging)
        {
            var (subOrganisationId, total, errors) = await _errors.List(submissionId, paging);

            // a few short circuit scenarios
            if (paging.Offset >= total && paging.Offset > 0)
                return BadRequest($"'offset' exceeds total of {total} records"); //Is this really bad request?

            // if (!User.HasClaim(CustomClaimTypes.OrganisationId,
            //     subOrganisationId.ToString()))
            //     return Forbid();

            if (total == 0)
                return Ok(new PaginatedErrorsDto()
                {
                    Errors = new List<ErrorDto>()
                });

            // Prepare the return model
            var model = new PaginatedErrorsDto
            {
                Errors = _mapper.Map<ICollection<ErrorDto>>(errors)
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
        [SwaggerResponse(200, Type = typeof(ErrorDto))]
        [SwaggerResponse(403, "Access to the requested submission denied.")]
        [SwaggerResponse(404, "No error found with the specified id matching the specified submission.")]
        public async Task<IActionResult> GetError(int submissionId, int errorId)
        {
            var error = await _errors.Get(errorId);

            if (error?.Submission.Id != submissionId) return NotFound();

           // if (!User.HasClaim(CustomClaimTypes.OrganisationId,
           //    error.Submission.OrganisationId.ToString()))
           //     return Forbid();

            return Ok(_mapper.Map<ErrorDto>(error));
        }
    }
}
