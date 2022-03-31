using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Submissions.Api.Models.ADAC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;


namespace Biobanks.Submissions.Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class AccessConditionController : ControllerBase
    {
        private readonly IReferenceDataService _accessConditionService;

        public AccessConditionController(IReferenceDataService accessConditionService)
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
        [SwaggerResponse(200 , Type = typeof(ReadAccessConditionsModel))]
        public async Task<IList> Get()
        {
            var models = (await _accessConditionService.List())
                .Select(x =>
                    Task.Run(async () => new ReadAccessConditionsModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        AccessConditionCount = await _accessConditionService.GetUsageCount(x.Id),
                    }
                    )
                    .Result
                )
                .ToList();
            return models;
        }

    }

}