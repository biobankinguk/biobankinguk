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
    public class SampleCollectionModeController : ControllerBase
    {
        private readonly IReferenceDataService<SampleCollectionMode> _sampleCollectionModeService;

        public SampleCollectionModeController(IReferenceDataService<SampleCollectionMode> sampleCollectionModeService)
        {
            _sampleCollectionModeService = sampleCollectionModeService;
        }
        /// <summary>
        /// Generate sample collection list
        /// </summary>
        /// <returns>A list of sample collection</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(SampleCollectionModeModel))]
        public async Task<IList> Get()
        {
            var models = (await _sampleCollectionModeService.List())
                .Select(x =>
                    Task.Run(() => new SampleCollectionModeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder
                    })
                )
                .ToList();

            return models;
        }
    }
}
