using System.Threading.Tasks;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Services.Submissions.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Directory.Controllers.Submissions.Domain
{
    /// <inheritdoc />
    /// <summary>
    /// This controller serves as a sub-controller for rejecting submissions.
    /// </summary>
    [Route("{biobankId}/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Submissions")]
    [Authorize(nameof(AuthPolicies.IsTokenAuthenticated))]
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
