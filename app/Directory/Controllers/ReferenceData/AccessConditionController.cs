using Biobanks.Entities.Data.ReferenceData;
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
    public class AccessConditionController : ControllerBase
    {
        private readonly IReferenceDataCrudService<AccessCondition> _accessConditionService;

        public AccessConditionController(IReferenceDataCrudService<AccessCondition> accessConditionService)
        {
            _accessConditionService = accessConditionService;
        }

        /// <summary>
        /// Generate access condition list
        /// </summary>
        /// <returns>A list of access conditions</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200 , Type = typeof(AccessConditionModel))]
        public async Task<IList> Get()
        {
            var models = (await _accessConditionService.List())
                .Select(x =>
                    Task.Run(() => Task.FromResult(new AccessConditionModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                    })

                    )
                    .Result
                )
                .ToList();
            return models;
        }

        /// <summary>
        /// Insert an Access Condition.
        /// </summary>
        /// <param name="model">The model to be inserted.</param>
        /// <returns>The created Access Condition.</returns>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(AccessConditionModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<ActionResult> Post(AccessConditionModel model)
        {
            //If this description is valid, it already exists
            if (await _accessConditionService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Access condition descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var access = new AccessCondition
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _accessConditionService.Add(access);
            await _accessConditionService.Update(access);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update an Access Condition.
        /// </summary>
        /// <param name="id">The ID of the condition to update.</param>
        /// <param name="model">The new model to be inserted.</param>
        /// <returns>The updated Access Condition.</returns>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(AccessConditionModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<ActionResult> Put(int id, AccessConditionModel model)
        {
            var existing = await _accessConditionService.Get(model.Description);

            //If this description is valid, it already exists
            if (existing != null && existing.Id != id)
            {
                ModelState.AddModelError("Description", "That description is already in use by another access condition. Access condition descriptions must be unique.");
            }


            // If in use, prevent update
            if (await _accessConditionService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The access condition \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var access = new AccessCondition
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _accessConditionService.Update(access);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Delete an access condition.
        /// </summary>
        /// <param name="id">The ID of the condition to update.</param>
        /// <returns>Deleted access condition.</returns>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(AccessConditionModel))]
        [SwaggerResponse(400, "Invalid request body.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _accessConditionService.Get(id);

            // If in use, prevent update
            if (await _accessConditionService.IsInUse(id))
            {
                ModelState.AddModelError("Description", $"The access condition \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _accessConditionService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Move an Access Condition.
        /// </summary>
        /// <param name="id">The ID of the condition to move.</param>
        /// <param name="model">The new model to update.</param>
        /// <returns>The updated Access Condition.</returns>
        [HttpPost("{id}/move")]
        [SwaggerResponse(200, Type = typeof(AccessConditionModel))]
        public async Task<ActionResult> Move(int id, AccessConditionModel model)
        {
            var access = new AccessCondition
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _accessConditionService.Update(access);

            //Everything went A-OK!
            return Ok(model);
        }

    }

}
