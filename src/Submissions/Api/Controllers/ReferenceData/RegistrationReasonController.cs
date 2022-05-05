using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class RegistrationReasonController : ControllerBase
    {
        private readonly IReferenceDataService<RegistrationReason> _registrationReasonService;

        public RegistrationReasonController(IReferenceDataService<RegistrationReason> registrationReasonService)
        {
            _registrationReasonService = registrationReasonService;
        }
        /// <summary>
        /// Generate registration reason list
        /// </summary>
        /// <returns>A list of registration reason</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(RegistrationReasonModel))]
        public async Task<IList> Get()
        {
            var model = (await _registrationReasonService.List())
                .Select(x => new RegistrationReasonModel
            {
                Id = x.Id,
                Description = x.Value
            })
            .ToList();
            return model;
        }
    }
}
