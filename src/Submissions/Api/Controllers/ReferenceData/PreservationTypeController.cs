using Biobanks.Entities.Shared.ReferenceData;
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
    public class PreservationTypeController : ControllerBase
    {
        private readonly IReferenceDataService<PreservationType> _preservationTypeService;

        public PreservationTypeController(IReferenceDataService<PreservationType> preservationTypeService)
        {
            _preservationTypeService = preservationTypeService;
        }
        /// <summary>
        /// Generate preservation type list
        /// </summary>
        /// <returns>A list of preservation type</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(PreservationTypeModel))]
        public async Task<IList> Get()
        {
            var model = (await _preservationTypeService.List())
                .Select(x =>
                    new PreservationTypeModel()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                        StorageTemperatureId = x.StorageTemperatureId,
                        StorageTemperatureName = x.StorageTemperature?.Value ?? ""
                    }
                )
                .ToList();

            return model;
        }
    }
}
