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
    public class AssociatedDataProcurementTimeFrameController : ControllerBase
    {
        private readonly IReferenceDataService<AssociatedDataProcurementTimeframe> _associatedDataProcurementTimeFrameService;

        public AssociatedDataProcurementTimeFrameController(
            IReferenceDataService<AssociatedDataProcurementTimeframe> associatedDataProcurementTimeFrameService)
        {
            _associatedDataProcurementTimeFrameService = associatedDataProcurementTimeFrameService;
        }
        /// <summary>
        /// Generate associate data procurement time frame
        /// </summary>
        /// <returns>A list associate data procurement time frame</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AssociatedDataProcurementTimeFrameModel))]
        public async Task<IList> Get()
        {
            var models = (await _associatedDataProcurementTimeFrameService.List())
                .Select(x => new AssociatedDataProcurementTimeFrameModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        DisplayName = x.DisplayValue,
                        SortOrder = x.SortOrder
                    }
                )
                .ToList();
            return models;
        }
    }
}
