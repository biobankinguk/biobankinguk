using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Services.Contracts;
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
        private readonly IBackgroundJobEnqueueingService _backgroundJobEnqueueingService;

        /// <inheritdoc />
        public RejectController(IBackgroundJobEnqueueingService backgroundJobEnqueueingService)
        {
            _backgroundJobEnqueueingService = backgroundJobEnqueueingService;
        }

        /// <summary>
        /// Rejects all staged data changes.
        /// </summary>
        /// <param name="biobankId">Biobank ID to operate on.</param>
        [HttpPost]
        [SwaggerResponse(204, Description = "The reject completed successfully.")]
        [SwaggerResponse(400, Description = "Organisation ID claim in bad format.")]
        public async Task<IActionResult> PostAsync(int biobankId)
        {
            if (!User.HasClaim(CustomClaimTypes.BiobankId,
                biobankId.ToString()))
                return Forbid();

            await _backgroundJobEnqueueingService.Reject(biobankId);

            return NoContent();
        }
    }
}
