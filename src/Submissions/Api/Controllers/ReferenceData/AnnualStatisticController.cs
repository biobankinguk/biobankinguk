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
    public class AnnualStatisticController : ControllerBase
    {
        private readonly IReferenceDataService<AnnualStatistic> _annualStatisticService;
        private readonly IReferenceDataService<AnnualStatisticGroup> _annualStatisticGroupService;

        public AnnualStatisticController(
            IReferenceDataService<AnnualStatistic> annualStatisticService,
            IReferenceDataService<AnnualStatisticGroup> annualStatisticGroupService)
        {
            _annualStatisticService = annualStatisticService;
            _annualStatisticGroupService = annualStatisticGroupService;
        }
        /// <summary>
        /// Generate annual statistic list
        /// </summary>
        /// <returns>A list of annual statistics</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AnnualStatisticModel))]
        public async Task<IActionResult> Get()
        {
            var groups = (await _annualStatisticGroupService.List())
                 .Select(x => new AnnualStatisticGroupModel
                 {
                     AnnualStatisticGroupId = x.Id,
                     Name = x.Value,
                 })
                 .ToList();
            var models = (await _annualStatisticService.List())
                .Select(x =>
                    Task.Run(async () => new AnnualStatisticModel
                    {
                        Id = x.Id,
                        Name = x.Value,
                        AnnualStatisticGroupId = x.AnnualStatisticGroupId,
                        AnnualStatisticGroupName = groups.Where(y => y.AnnualStatisticGroupId == x.AnnualStatisticGroupId).FirstOrDefault()?.Name,
                    })
                    .Result
                )
                .ToList();

            return new JsonResult(new
            {
                AnnualStatistics = models,
                AnnualStatisticGroups = groups,
            });


        }
    }
}
