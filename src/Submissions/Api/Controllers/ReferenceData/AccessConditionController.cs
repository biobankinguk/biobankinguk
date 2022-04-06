using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Shared.Services.Contracts;
using Biobanks.Submissions.Api.Models.ADAC;
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
    public class AccessConditionController : ControllerBase
    {
        private readonly IReferenceDataService<AccessCondition> _accessConditionService;

        public AccessConditionController(IReferenceDataService<AccessCondition> accessConditionService)
        {
            _accessConditionService = accessConditionService;
        }
        /// <summary>
        /// Generate access condition list
        /// </summary>
        /// <returns>A list of access conditions</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200 , Type = typeof(AccessConditionModel))]
        public async Task<IList> Get()
        {
            var models = (await _accessConditionService.List())
                .Select(x =>
                    Task.Run(async () => new AccessConditionModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                    }
                    )
                    .Result
                )
                .ToList();
            return models;
        }

    }

}