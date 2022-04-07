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
    public class AgeRangeController : ControllerBase
    {
        private readonly IReferenceDataService<AgeRange> _ageRangeService;

        public AgeRangeController(IReferenceDataService<AgeRange> ageRangeService)
        {
            _ageRangeService = ageRangeService;
        }
        /// <summary>
        /// Generate age range list
        /// </summary>
        /// <returns>A list of age ranges</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(AgeRangeModel))]
        public async Task<IList> Get()
        {
            var models = (await _ageRangeService.List())
                .Select(x =>
                    Task.Run(async () => new AgeRangeModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound
                    })
                    .Result
                )
                .ToList();

            return models;
        }
    }
}
