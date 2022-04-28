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
    public class StorageTemperatureController : ControllerBase
    {
        private readonly IReferenceDataService<StorageTemperature> _storageTemperatureService;
        public StorageTemperatureController(
            IReferenceDataService<StorageTemperature> storageTemperatureService)
        {
            _storageTemperatureService = storageTemperatureService;
        }
        /// <summary>
        /// Generate storage temperature list
        /// </summary>
        /// <returns>A list of storage temperature</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(StorageTemperatureModel))]
        public async Task<IList> Get()
        {
            var models = (await _storageTemperatureService.List())
                .Select(x =>
                    Task.Run(() =>
                        new StorageTemperatureModel()
                        {
                            Id = x.Id,
                            Value = x.Value,
                            SortOrder = x.SortOrder,
                            /*IsInUse = await _storageTemperatureService.IsInUse(x.Id),
                            SampleSetsCount = await _storageTemperatureService.GetUsageCount(x.Id)*/
                        }
                    )
                )
                .ToList();

            return models;
        }
    }
}
