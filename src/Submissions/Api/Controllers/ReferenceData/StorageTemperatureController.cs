using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Services.Directory;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class StorageTemperatureController : ControllerBase
    {
        private readonly IReferenceDataService<PreservationType> _preservationTypeService;
        private readonly IReferenceDataService<StorageTemperature> _storageTemperatureService;
        private readonly IConfigService _configService;

        public StorageTemperatureController(
            IReferenceDataService<PreservationType> preservationTypeService,
            IReferenceDataService<StorageTemperature> storageTemperatureService,
            IConfigService configService)
        {
            _preservationTypeService = preservationTypeService;
            _storageTemperatureService = storageTemperatureService;
            _configService = configService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IList> Get()
        {
            var models = (await _storageTemperatureService.List())
                .Select(x =>
                    Task.Run(async () =>
                        new StorageTemperatureModel()
                        {
                            Id = x.Id,
                            Value = x.Value,
                            SortOrder = x.SortOrder,
                            IsInUse = await _storageTemperatureService.IsInUse(x.Id),
                            SampleSetsCount = await _storageTemperatureService.GetUsageCount(x.Id)
                        }
                    ).Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post(StorageTemperatureModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.StorageTemperatureName);

            // Validate model
            if (await _storageTemperatureService.Exists(model.Value))
            {
                ModelState.AddModelError("StorageTemperature", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = new StorageTemperature
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder
            };

            await _storageTemperatureService.Add(type);
            await _storageTemperatureService.Update(type); // Ensure sortOrder is correct

            // Success response
            return Ok(model);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Put(int id, StorageTemperatureModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.StorageTemperatureName);

            // Validate model
            if (await _storageTemperatureService.Exists(model.Value))
            {
                ModelState.AddModelError("StorageTemperature", $"That {currentReferenceName.Value} already exists!");
            }

            // If in use, prevent update
            if (model.IsInUse)
            {
                ModelState.AddModelError("StorageTemperature", $"The storage temperature \"{model.Value}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _storageTemperatureService.Update(new StorageTemperature
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder
            });

            // Success message
            return Ok(model);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _storageTemperatureService.Get(id);

            // If in use, prevent update
            if (await _storageTemperatureService.IsInUse(id))
            {
                ModelState.AddModelError("StorageTemperature", $"The storage temperature \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _storageTemperatureService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        [HttpPost("{id}/move")]
        [AllowAnonymous]
        public async Task<ActionResult> Move(int id, StorageTemperatureModel model)
        {
            await _storageTemperatureService.Update(new StorageTemperature
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }

        [HttpGet("{storageTemperature}/preservationtype")]
        [AllowAnonymous]
        public async Task<IList> GetValidPreservationTypes(int storageTemperature)
            => (await _preservationTypeService.List())
                .Where(x => x.StorageTemperatureId == storageTemperature)
                .Select(x => x.Id)
                .ToList();
    }
}

