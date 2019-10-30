using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upload.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// This controller serves as a sub-controller for rejecting submissions.
    /// </summary>
    [Route("{biobankId}/[controller]")]
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
        /// <param name="biobankId">Biobank ID to operate on.</param>
        [HttpPost]
        [SwaggerResponse(204, Description = "The reject completed successfully.")]
        [SwaggerResponse(400, Description = "Organisation ID claim in bad format.")]
        public IActionResult Post(int biobankId)
        {
          //  if (!User.HasClaim(CustomClaimTypes.BiobankId,
          //      biobankId.ToString()))
          //      return Forbid();

            //Hangfire stuff
            //TODO check if this is still suitable
            //BackgroundJob.Enqueue(() => _service.RejectStagedData(biobankId));

            return NoContent();
        }
    }
}
