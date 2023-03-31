using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.Shared.ReferenceData;
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
    public class MaterialTypeGroupController : ControllerBase
    {
        private readonly IReferenceDataCrudService<MaterialTypeGroup> _materialTypeGroupService;

        public MaterialTypeGroupController(IReferenceDataCrudService<MaterialTypeGroup> materialTypeGroupService)
        {
            _materialTypeGroupService = materialTypeGroupService;
        }

        /// <summary>
        /// Generate a Material Type Group list.
        /// </summary>
        /// <returns>List of Material Group Types.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(MaterialTypeGroupModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<IEnumerable> Get()
        {
            var materialTypeGroups = await _materialTypeGroupService.List();

            return materialTypeGroups.Select(x => new MaterialTypeGroupModel
            {
                Id = x.Id,
                Description = x.Value,
                MaterialTypes = x.MaterialTypes.Select(x => x.Value),
                MaterialTypeCount = x.MaterialTypes.Count()
            });
        }

        /// <summary>
        /// Insert a new Material Type Group.
        /// </summary>
        /// <param name="model">The model to insert.</param>
        /// <returns>The insert Material Type Group.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(MaterialTypeGroupModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(MaterialTypeGroupModel model)
        {
            if (await _materialTypeGroupService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Material Type Group descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _materialTypeGroupService.Add(new MaterialTypeGroup
            {
                Id = model.Id,
                Value = model.Description
            });

            return Ok(model);
        }

        /// <summary>
        /// Update a Material Type Group.
        /// </summary>
        /// <param name="id">Id of the model to update.</param>
        /// <param name="model">The new values to update with.</param>
        /// <returns>The updated Material Type Group.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(MaterialTypeGroupModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, MaterialTypeGroupModel model)
        {
            // Validate model
            if (await _materialTypeGroupService.Exists(model.Description))
            {
                ModelState.AddModelError("MaterialType", "That description is already in use. Material Type Groups must be unique.");
            }

            // If in use, prevent update
            if (await _materialTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type group \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _materialTypeGroupService.Update(new MaterialTypeGroup
            {
                Id = model.Id,
                Value = model.Description
            });

            return Ok(model);
        }

        /// <summary>
        /// Delete a Material Type Group.
        /// </summary>
        /// <param name="id">Id of the Material Type Group to delete.</param>
        /// <returns>The deleted Material Type Group.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(MaterialTypeGroupModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _materialTypeGroupService.Get(id);

            // If in use, prevent update
            if (await _materialTypeGroupService.IsInUse(id))
            {
                ModelState.AddModelError("MaterialType", $"The material type group \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _materialTypeGroupService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }
    }
}
