using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Common.Auth;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.SubmissionApi.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class MaterialTypeGroupController : ControllerBase
    {
        private readonly IMaterialTypeGroupService _materialTypeGroupService;

        /// <inheritdoc />
        public MaterialTypeGroupController(IMaterialTypeGroupService materialTypeGroupService)
        {
            _materialTypeGroupService = materialTypeGroupService;
        }

        /// <summary>
        /// Lists all the available Material Type Group entities.
        /// </summary>
        /// <returns>A list of Material Type Group entities.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<MaterialTypeGroup>))]
        public async Task<IActionResult> Get() => Ok(await _materialTypeGroupService.List());

        /// <summary>
        /// Gets a specific Material Type Group entity.
        /// </summary>
        /// <param name="id">The ID of the Material Type Group entity to return.</param>
        /// <returns>A single Material Type Group entity.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(MaterialTypeGroup))]
        [SwaggerResponse(404, "Material Type Group with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var materialTypeGroup = await _materialTypeGroupService.Get(id);
            if (materialTypeGroup != null)
                return Ok(materialTypeGroup);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Material Type Group entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Material Type Group to add or update.</param>
        /// <param name="model">Material Type Group object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] MaterialTypeGroup model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _materialTypeGroupService.Get(id) == null)
            {
                await _materialTypeGroupService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _materialTypeGroupService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds a Material Type Group entity with an auto-generated identity.
        /// </summary>
        /// <param name="value">Material Type Group value to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            //check we're maintaining unique values
            if (await _materialTypeGroupService.GetByValue(value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, value);

            var materialTypeGroup = await _materialTypeGroupService.Create(new MaterialTypeGroup{Value = value});

            return CreatedAtAction("Get", new { id = materialTypeGroup.Id }, materialTypeGroup);
        }
    }
}
