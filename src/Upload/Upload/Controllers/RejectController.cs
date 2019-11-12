using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Upload.Contracts;

namespace Upload.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// This controller serves as a sub-controller for rejecting submissions.
    /// </summary>
    [Route("{organisationId}/[controller]")]
    [ApiController]
    public class RejectController : Controller
    {
        private readonly IRejectService _service;

        /// <inheritdoc />
        public RejectController(IRejectService service)
        {
            _service = service;
        }

        /// <summary>
        /// Rejects all staged data changes.
        /// </summary>
        /// <param name="organisationId">Organisation ID to operate on.</param>
        [HttpPost]
        [SwaggerResponse(204, Description = "The reject completed successfully.")]
        [SwaggerResponse(400, Description = "Organisation ID claim in bad format.")]
        public IActionResult Post(int organisationId)
        {
          //  if (!User.HasClaim(CustomClaimTypes.OrganisationId,
          //      organisationId.ToString()))
          //      return Forbid();

            //Hangfire stuff
            BackgroundJob.Enqueue(() => _service.RejectStagedData(organisationId));

            return NoContent();
        }
    }
}
