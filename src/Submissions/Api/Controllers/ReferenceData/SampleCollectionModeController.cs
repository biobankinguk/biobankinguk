using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data.ReferenceData;

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

        [HttpGet]
        [AllowAnonymous]
        public async Task<IList> Get()
        {
            var models = (await _sampleCollectionModeService.List())
                .Select(x =>
                    Task.Run(async () => new SampleCollectionModeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _sampleCollectionModeService.GetUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post(SampleCollectionModeModel model)
        {
            //// Validate model
            if (await _sampleCollectionModeService.Exists(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That description is already in use. Sample collection modes must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mode = new SampleCollectionMode
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _sampleCollectionModeService.Add(mode);
            await _sampleCollectionModeService.Update(mode);

            // Success response
            return Ok(model);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Put(int id, SampleCollectionModeModel model)
        {
            // Validate model
            if (await _sampleCollectionModeService.Exists(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That sample collection modes already exists!");
            }

            if (await _sampleCollectionModeService.IsInUse(id))
            {
                ModelState.AddModelError("SampleCollectionModes", $"This sample collection mode \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mode = new SampleCollectionMode
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _sampleCollectionModeService.Update(mode);

            // Success message
            return Ok(model);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _sampleCollectionModeService.Get(id);

            if (await _sampleCollectionModeService.IsInUse(id))
            {
                ModelState.AddModelError("SampleCollectionModes", $"This sample collection mode \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _sampleCollectionModeService.Delete(id);

            // Success
            return Ok(model);
        }

        [HttpPost("{id}/move")]
        [AllowAnonymous]
        public async Task<ActionResult> Move(int id, SampleCollectionModeModel model)
        {
            var mode = new SampleCollectionMode
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _sampleCollectionModeService.Update(mode);

            //Everything went A-OK!
            return Ok(model);
        }
    }
}

