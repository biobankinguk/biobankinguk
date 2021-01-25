using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Common.Auth;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.SubmissionApi.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class SnomedTagController : ControllerBase
    {
        private readonly ISnomedTagService _snomedTagService;

        /// <inheritdoc />
        public SnomedTagController(ISnomedTagService snomedTagService)
        {
            _snomedTagService = snomedTagService;
        }

        /// <summary>
        /// Lists all the available Snomed Tag entities.
        /// </summary>
        /// <returns>A list of Snomed Tag entities.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<SnomedTag>))]
        public async Task<IActionResult> Get() => Ok(await _snomedTagService.List());

        /// <summary>
        /// Gets a specific Snomed Tag entity.
        /// </summary>
        /// <param name="id">The ID of the Snomed Tag entity to return.</param>
        /// <returns>A single Snomed Tag entity.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(SnomedTag))]
        [SwaggerResponse(404, "Snomed Tag with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var snomedTag = await _snomedTagService.Get(id);
            if (snomedTag != null)
                return Ok(snomedTag);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Snomed Tag entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Snomed Tag to add or update.</param>
        /// <param name="model">Snomed Tag object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] SnomedTag model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _snomedTagService.Get(id) == null)
            {
                await _snomedTagService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _snomedTagService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds or updates a Snomed Tag entity with an auto-generated identity.
        /// </summary>
        /// <param name="value">Snomed Tag name value to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            //check we're maintaining unique values
            if (await _snomedTagService.GetByValue(value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, value);

            var snomedTag = await _snomedTagService.Create(new SnomedTag{Value = value});

            return CreatedAtAction("Get", new { id = snomedTag.Id }, snomedTag);
        }
    }
}
