using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
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
    public class MaterialTypeGroupController : ControllerBase
    {
        private readonly IReferenceDataService<MaterialTypeGroup> _materialTypeGroupService;

        public MaterialTypeGroupController(IReferenceDataService<MaterialTypeGroup> materialTypeGroupService)
        {
            _materialTypeGroupService = materialTypeGroupService;
        }

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

        [HttpPost]
        [AllowAnonymous]
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

        [HttpPut("{id}")]
        [AllowAnonymous]
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

        [HttpDelete("{id}")]
        [AllowAnonymous]
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
