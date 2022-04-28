using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Web.Models.Shared;
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
    public class ConsentRestrictionController : ControllerBase
    {
        private readonly IReferenceDataService<ConsentRestriction> _consentRestrictionService;

        public ConsentRestrictionController(IReferenceDataService<ConsentRestriction> consentRestrictionService)
        {
            _consentRestrictionService = consentRestrictionService;
        }
        /// <summary>
        /// Generate consent restriction list
        /// </summary>
        /// <returns>A list of consent restriction</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ConsentRestrictionModel))]
        public async Task<IList> Get()
        {
            var model = (await _consentRestrictionService.List())
                    .Select(x =>

                Task.Run(() => new ConsentRestrictionModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    SortOrder = x.SortOrder
                }))

                    .ToList();

            return model;
        }

    }
}
