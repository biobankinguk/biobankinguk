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
    public class AnnualStatisticGroupController : ControllerBase
    {
        private readonly IReferenceDataService<AnnualStatisticGroup> _annualStatisticGroupService;


        public AnnualStatisticGroupController(IReferenceDataService<AnnualStatisticGroup> annualStatisticGroupService)
        {
            _annualStatisticGroupService = annualStatisticGroupService;
        }
        /// <summary>
        /// Generate annual statistic group list
        /// </summary>
        /// <returns>A list of annual statistic groups</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticGroupModel))]
        public async Task<IList> Get()
        {
            var model = (await _annualStatisticGroupService.List())
                .Select(x => new AnnualStatisticGroupModel
                {
                    AnnualStatisticGroupId = x.Id,
                    Name = x.Value,
                })
                .ToList();
            return model;
        }
    }
}
