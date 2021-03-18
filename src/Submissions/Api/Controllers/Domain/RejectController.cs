using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Services.Contracts;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Submissions.Api.Controllers.Domain
{
    /// <inheritdoc />
    /// <summary>
    /// This controller serves as a sub-controller for rejecting submissions.
    /// </summary>
    [Route("{biobankId}/[controller]")]
    [ApiController]
    public class RejectController : ControllerBase
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
        /// <param name="biobankId">Biobank ID to operate on.</param>
        [HttpPost]
        [SwaggerResponse(204, Description = "The reject completed successfully.")]
        [SwaggerResponse(400, Description = "Organisation ID claim in bad format.")]
        public IActionResult Post(int biobankId)
        {
            if (!User.HasClaim(CustomClaimTypes.BiobankId,
                biobankId.ToString()))
                return Forbid();

            BackgroundJob.Enqueue(() => _service.RejectStagedData(biobankId));

            return NoContent();
        }
    }
}
