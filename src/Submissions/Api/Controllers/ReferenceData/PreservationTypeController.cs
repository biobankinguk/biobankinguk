using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class PreservationTypeController : ControllerBase
    {
        private readonly IReferenceDataCrudService<PreservationType> _preservationTypeService;

        public PreservationTypeController(IReferenceDataCrudService<PreservationType> preservationTypeService)
        {
            _preservationTypeService = preservationTypeService;
        }

        /// <summary>
        /// Generate a preservation type list.
        /// </summary>
        /// <returns>List of preservation types.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(PreservationTypeModel))]
        public async Task<IList> Get()
        {
            var model = (await _preservationTypeService.List())
                .Select(x =>
                    new PreservationTypeModel()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                        StorageTemperatureId = x.StorageTemperatureId,
                        StorageTemperatureName = x.StorageTemperature?.Value ?? ""
                    }
                )
                .ToList();

            return model;
        }

        /// <summary>
        /// Insert new preservation type.
        /// </summary>
        /// <param name="model">Model of preservation type to insert.</param>
        /// <returns></returns>
        /// <response code="200">Request Accepted</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(PreservationTypeModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(PreservationTypeModel model)
        {
            var existing = await _preservationTypeService.Get(model.Value);

            if (existing != null && existing.Id != model.Id)
            {
                ModelState.AddModelError("PreservationTypes", $"That PresevationType is already in use. '{model.Value}' is already in use at the StorageTemperature.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var type = new PreservationType
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                StorageTemperatureId = model.StorageTemperatureId
            };

            await _preservationTypeService.Add(type);
            await _preservationTypeService.Update(type); // Ensure sortOrder is correct

            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Update existing preservation type.
        /// </summary>
        /// <param name="id">ID of the preservation type to update.</param>
        /// <param name="model">Model of the values to update with.</param>
        /// <returns>The updated preservation type model.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(PreservationTypeModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, PreservationTypeModel model)
        {
            var existing = await _preservationTypeService.Get(model.Value);

            if (existing != null && existing.Id != model.Id)
            {
                ModelState.AddModelError("PreservationTypes", $"That PreservationType is already in use. '{model.Value}' is already in use at the StorageTemperature.");
            }

            if (await _preservationTypeService.IsInUse(model.Id))
            {
                ModelState.AddModelError("PreservationTypes", $"Unable to change '{model.Value}', as it is currently is use.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _preservationTypeService.Update(new PreservationType()
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                StorageTemperatureId = model.StorageTemperatureId
            });

            // Success message
            return Ok(model);
        }

        /// <summary>
        /// Delete existing preservation type.
        /// </summary>
        /// <param name="id">ID of the preservation type to delete.</param>
        /// <returns>The deleted preservation type.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(PreservationTypeModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _preservationTypeService.Get(id);

            // If in use, prevent update
            if (await _preservationTypeService.IsInUse(id))
            {
                ModelState.AddModelError("PreservationTypes", $"Unable to delete '{model.Value}', as it is currently is use.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _preservationTypeService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Move an existing preservation type.
        /// </summary>
        /// <param name="id">ID of the preservation type to move.</param>
        /// <param name="model">Model of the values to update with.</param>
        /// <returns>The updated preservation type.</returns>
        /// <response code="200">Request Accepted</response>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(PreservationTypeModel))]
        public async Task<ActionResult> Move(int id, PreservationTypeModel model)
        {
            await _preservationTypeService.Update(new PreservationType()
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                StorageTemperatureId = model.StorageTemperatureId
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}

