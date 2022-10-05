using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Biobanks.Submissions.Api.Services.Directory;
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
    public class PreservationTypeController : ControllerBase
    {
        private readonly IReferenceDataService<PreservationType> _preservationTypeService;

        public PreservationTypeController(IReferenceDataService<PreservationType> preservationTypeService)
        {
            _preservationTypeService = preservationTypeService;
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpPost]
        [AllowAnonymous]
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
            return Accepted(model);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
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
            return Accepted(model);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
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
            return Accepted(model);
        }

        [HttpPost("{id}/move")]
        [AllowAnonymous]
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
            return Accepted(model);
        }
    }
}

