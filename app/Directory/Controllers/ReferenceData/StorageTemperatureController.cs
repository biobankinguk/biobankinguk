using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Shared.ReferenceData;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Directory.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class StorageTemperatureController : ControllerBase
    {
        private readonly IReferenceDataCrudService<PreservationType> _preservationTypeService;
        private readonly IReferenceDataCrudService<StorageTemperature> _storageTemperatureService;
        private readonly IConfigService _configService;

        public StorageTemperatureController(
            IReferenceDataCrudService<PreservationType> preservationTypeService,
            IReferenceDataCrudService<StorageTemperature> storageTemperatureService,
            IConfigService configService)
        {
            _preservationTypeService = preservationTypeService;
            _storageTemperatureService = storageTemperatureService;
            _configService = configService;
        }

        /// <summary>
        /// Generate a list of Storage Temperatures.
        /// </summary>
        /// <returns>List of Storage Temperatures.</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(StorageTemperatureModel))]
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

        /// <summary>
        /// Insert a new Storage Temperature.
        /// </summary>
        /// <param name="model">Model to insert.</param>
        /// <returns>The inserted Storage Temperature.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(StorageTemperatureModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(StorageTemperatureModel model)
        {
            //Getting the name of the reference type as stored in the config
            Data.Entities.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.StorageTemperatureName);

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

        /// <summary>
        /// Update a Storage Temperature.
        /// </summary>
        /// <param name="id">Id of the Storage Temperature to update.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated Storage Temperature.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(StorageTemperatureModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, StorageTemperatureModel model)
        {
            //Getting the name of the reference type as stored in the config
            Data.Entities.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.StorageTemperatureName);

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

        /// <summary>
        /// Delete an existing Storage Temperature.
        /// </summary>
        /// <param name="id">Id of the Storage Temperature to delete.</param>
        /// <returns>Deleted Storage Temperature.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(StorageTemperature))]
        [SwaggerResponse(400, "Invalid request.")]
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

        /// <summary>
        /// Move a Storage Temperature.
        /// </summary>
        /// <param name="id">Id of the Storage Temperature to move.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated Storage Temperature.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(StorageTemperatureModel))]
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

        /// <summary>
        /// Gets Preservation Types that use a Storage Temperature.
        /// </summary>
        /// <param name="storageTemperature">Name of the storage temperature.</param>
        /// <returns>List of Ids of matching Preservation Types.</returns>
        [HttpGet("{storageTemperature}/preservationtype")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(IEnumerable))]
        public async Task<IList> GetValidPreservationTypes(int storageTemperature)
            => (await _preservationTypeService.List())
                .Where(x => x.StorageTemperatureId == storageTemperature)
                .Select(x => x.Id)
                .ToList();
    }
}

