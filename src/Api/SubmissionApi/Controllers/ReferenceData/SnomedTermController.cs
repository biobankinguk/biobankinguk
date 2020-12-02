using System.Collections.Generic;
using System.Linq;
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
    public class SnomedTermController : ControllerBase
    {
        private readonly ISnomedTermService _snomedTermService;

        /// <inheritdoc />
        public SnomedTermController(ISnomedTermService snomedTermService)
        {
            _snomedTermService = snomedTermService;
        }

        /// <summary>
        /// Lists all the available Snomed Terms.
        /// </summary>
        /// <returns>A list of Snomed Terms.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<SnomedTerm>))]
        public async Task<IActionResult> Get() => Ok(await _snomedTermService.List());

        /// <summary>
        /// Gets a specific Snomed Term.
        /// </summary>
        /// <param name="id">The ID of the Snomed Term to return.</param>
        /// <returns>A single Snomed Term.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(SnomedTag))]
        [SwaggerResponse(404, "Snomed Term with provided id does not exist.")]
        public async Task<IActionResult> Get(string id)
        {
            var snomedTerm = await _snomedTermService.Get(id);
            if (snomedTerm != null)
                return Ok(snomedTerm);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Snomed Term entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Snomed Term to add or update.</param>
        /// <param name="model">Snomed Term object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(string id, [FromBody] SnomedTerm model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _snomedTermService.Get(id) == null)
            {
                await _snomedTermService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _snomedTermService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds or updates a Snomed Term entity with an auto-generated identity.
        /// </summary>
        /// <param name="model">Snomed Term object to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] SnomedTerm model)
        {
            //check we're maintaining unique values
            if (await _snomedTermService.GetByValue(model.Description) != null)
                return StatusCode((int)HttpStatusCode.Conflict, model.Description);

            await _snomedTermService.Create(model);

            return CreatedAtAction("Get", new { id = model.Id }, model);
        }

        /// <summary>
        /// Adds or updates a collection of Snomed Term entities with a pre-defined identity.
        /// </summary>
        /// <param name="models">Snomed Term objects to add or update.</param>
        /// <returns></returns>
        [HttpPost("batch")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> PostBatch([FromBody] IList<SnomedTerm> models)
        {
            if (models == null || models.Count == 0) return BadRequest("Request body expected.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updates = new List<SnomedTerm>();

            //separate lists for creation and update
            var inArray = models.Select(x => x.Id).ToArray();
            var existing = (await _snomedTermService.List())
                .Where(x => inArray.Contains(x.Id)).ToList();

            existing.ForEach(x =>
            {
                var m = models.Single(y => y.Id == x.Id);
                models.Remove(m);
                updates.Add(m);
            });

            await _snomedTermService.Create(models);
            await _snomedTermService.Update(updates);
            return Ok();
        }
    }
}
