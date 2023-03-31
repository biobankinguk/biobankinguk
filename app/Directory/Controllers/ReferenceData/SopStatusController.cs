using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Directory.Auth;
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
    public class SopStatusController : ControllerBase
    {
        private readonly IReferenceDataCrudService<SopStatus> _sopStatusService;

        public SopStatusController(IReferenceDataCrudService<SopStatus> sopStatusService)
        {
            _sopStatusService = sopStatusService;
        }

        /// <summary>
        /// Generate a list of SOP Status.
        /// </summary>
        /// <returns>The list of SOP Status.</returns>
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

        /// <summary>
        /// Insert a new SOP Status.
        /// </summary>
        /// <param name="model">The model to insert.</param>
        /// <returns>The inserted model.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(SopStatusModel))]
        [SwaggerResponse(400, "Invalid request.")]
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

        /// <summary>
        /// Update a SOP Status.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated model.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(SopStatusModel))]
        [SwaggerResponse(400, "Invalid request.")]
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

        /// <summary>
        /// Delete an existing SOP Status.
        /// </summary>
        /// <param name="id">Id of the SOP Status to delete.</param>
        /// <returns>The deleted SOP Status.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(SopStatus))]
        [SwaggerResponse(400, "Invalid request.")]
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

        /// <summary>
        /// Move an existing SOP Status.
        /// </summary>
        /// <param name="id">Id of the SOP Status to move.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated model.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(SopStatusModel))]
        [SwaggerResponse(400, "Invalid request.")]
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

