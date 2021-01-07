using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Common.Auth;
using Biobanks.Common.Models;
using Biobanks.SubmissionApi.Services.Contracts;
using Entities.Api.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.SubmissionApi.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class MaterialTypeController : ControllerBase
    {
        private readonly IMaterialTypeService _materialTypeService;

        /// <inheritdoc />
        public MaterialTypeController(IMaterialTypeService materialTypeService)
        {
            _materialTypeService = materialTypeService;
        }

        /// <summary>
        /// Lists all the available Material Type entities.
        /// </summary>
        /// <returns>A list of Material Type entities.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<MaterialType>))]
        public async Task<IActionResult> Get() => Ok(await _materialTypeService.List());

        /// <summary>
        /// Gets a specific Material Type entity.
        /// </summary>
        /// <param name="id">The ID of the Material Type entity to return.</param>
        /// <returns>A single Material Type entity.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(MaterialType))]
        [SwaggerResponse(404, "Material Type with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var materialType = await _materialTypeService.Get(id);
            if (materialType != null)
                return Ok(materialType);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Material Type entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Material Type to add or update.</param>
        /// <param name="model">Material Type object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] MaterialType model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _materialTypeService.Get(id) == null)
            {
                await _materialTypeService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _materialTypeService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds or updates a Material Type entity with an auto-generated identity.
        /// </summary>
        /// <param name="model">Material Type object to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] MaterialType model)
        {
            //check we're maintaining unique values
            if (await _materialTypeService.GetByValue(model.Value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, model.Value);

            await _materialTypeService.Create(model);

            return CreatedAtAction("Get", new { id = model.Id }, model);
        }

        /// <summary>
        /// Adds a collection of Material Type entities with an auto-generated identity.
        /// </summary>
        /// <param name="models">Material Type objects to add or update.</param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        [HttpPost("batch")]
        public async Task<IActionResult> PostBatch([FromBody] IList<MaterialTypeJsonModel> models)
        {
            //check we're maintaining unique values
            //allow the insertion of valid ones to proceed anyway?
            //we return which ones were added, so a client could check which failed
            var inArray = models.Select(y => y.Value).ToArray();
            var existing = (await _materialTypeService.List())
                .Where(x => inArray.Contains(x.Value))
                .ToList();
            existing.ForEach(x =>
                models.Remove(models
                    .Single(y => y.Value == x.Value)));

            await _materialTypeService.Create(models);

            return Ok();
        }
    }
}
