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
using Biobanks.Submissions.Api.Services.Directory;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class SopStatusController : ControllerBase
    {
        private readonly IReferenceDataService<SopStatus> _sopStatusService;

        public SopStatusController(IReferenceDataService<SopStatus> sopStatusService)
        {
            _sopStatusService = sopStatusService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IList> Get()
        {
            var models = (await _sopStatusService.List())
                .Select(x =>
                    Task.Run(async () => new SopStatusModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _sopStatusService.GetUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();
            return models;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post(SopStatusModel model)
        {
            // Validate model
            if (await _sopStatusService.Exists(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That description is already in use. Sop status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = new SopStatus
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _sopStatusService.Add(status);
            await _sopStatusService.Update(status);

            // Success response
            return Ok(model);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Put(int id, SopStatusModel model)
        {
            // Validate model
            if (await _sopStatusService.Exists(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That sop status already exists!");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("SopStatus", $"The access condition \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _sopStatusService.Update(new SopStatus
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            // Success message
            return Ok(model);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _sopStatusService.Get(id);

            // If in use, prevent update
            if (await _sopStatusService.IsInUse(id))
            {
                ModelState.AddModelError("SopStatus", $"The access condition \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _sopStatusService.Delete(id);

            // Success
            return Ok(model);
        }

        [HttpPost("{id}/move")]
        [AllowAnonymous]
        public async Task<ActionResult> Move(int id, SopStatusModel model)
        {
            await _sopStatusService.Update(new SopStatus
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}

