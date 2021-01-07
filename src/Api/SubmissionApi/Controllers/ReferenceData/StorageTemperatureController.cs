using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Common.Auth;
using Entities.Api.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.SubmissionApi.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class StorageTemperatureController : ControllerBase
    {
        private readonly IStorageTemperatureService _storageTemperatureService;

        /// <inheritdoc />
        public StorageTemperatureController(IStorageTemperatureService storageTemperatureService)
        {
            _storageTemperatureService = storageTemperatureService;
        }

        /// <summary>
        /// Lists all the available Storage Temperatures.
        /// </summary>
        /// <returns>A list of Storage Temperatures.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<StorageTemperature>))]
        public async Task<IActionResult> Get() => Ok(await _storageTemperatureService.List());

        /// <summary>
        /// Gets a specific Storage Temperature.
        /// </summary>
        /// <param name="id">The ID of the Storage Temperature to return.</param>
        /// <returns>A single Storage Temperature.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(StorageTemperature))]
        [SwaggerResponse(404, "Storage Temperature with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var storageTemperature = await _storageTemperatureService.Get(id);
            if (storageTemperature != null)
                return Ok(storageTemperature);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Storage Temperature entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Storage Temperature to add or update.</param>
        /// <param name="model">Storage Temperature object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] StorageTemperature model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _storageTemperatureService.Get(id) == null)
            {
                await _storageTemperatureService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _storageTemperatureService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds or updates a Storage Temperature entity with an auto-generated identity.
        /// </summary>
        /// <param name="value">Storage Temperature value to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            //check we're maintaining unique values
            if (await _storageTemperatureService.GetByValue(value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, value);

            var storageTemp = await _storageTemperatureService.Create(new StorageTemperature{Value = value});

            return CreatedAtAction("Get", new { id = storageTemp.Id }, storageTemp);
        }
    }
}
