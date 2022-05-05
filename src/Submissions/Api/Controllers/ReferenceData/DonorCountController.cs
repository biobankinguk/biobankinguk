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
    public class DonorCountController : ControllerBase
    {
        private readonly IReferenceDataService<DonorCount> _donorCountService;

        public DonorCountController(
            IReferenceDataService<DonorCount> donorCountService)
        {
            _donorCountService = donorCountService;
        }
        /// <summary>
        /// Generate donor count list
        /// </summary>
        /// <returns>A list of donor count</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(DonorCountModel))]
        public async Task<IList> Get()
        {
            var models = (await _donorCountService.List())
                .Select(x => new DonorCountModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                    }
                )
                .ToList();

            return models;
        }
    }
}
