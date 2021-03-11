using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class OntologyTermController : ControllerBase
    {
        private readonly IOntologyTermService _ontologyTermService;

        /// <inheritdoc />
        public OntologyTermController(IOntologyTermService ontologyTermService)
        {
            _ontologyTermService = ontologyTermService;
        }

        /// <summary>
        /// Lists all the available Snomed Terms.
        /// </summary>
        /// <returns>A list of Snomed Terms.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<OntologyTerm>))]
        public async Task<IActionResult> Get() => Ok(await _ontologyTermService.List());

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
            var ontologyTerm = await _ontologyTermService.Get(id);
            if (ontologyTerm != null)
                return Ok(ontologyTerm);

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
        public async Task<IActionResult> Put(string id, [FromBody] OntologyTerm model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _ontologyTermService.Get(id) == null)
            {
                await _ontologyTermService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _ontologyTermService.Update(model);
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
        public async Task<IActionResult> Post([FromBody] OntologyTerm model)
        {
            //check we're maintaining unique values
            if (await _ontologyTermService.GetByValue(model.Value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, model.Value);

            await _ontologyTermService.Create(model);

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
        public async Task<IActionResult> PostBatch([FromBody] IList<OntologyTerm> models)
        {
            if (models == null || models.Count == 0) return BadRequest("Request body expected.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updates = new List<OntologyTerm>();

            //separate lists for creation and update
            var inArray = models.Select(x => x.Id).ToArray();
            var existing = (await _ontologyTermService.List())
                .Where(x => inArray.Contains(x.Id)).ToList();

            existing.ForEach(x =>
            {
                var m = models.Single(y => y.Id == x.Id);
                models.Remove(m);
                updates.Add(m);
            });

            await _ontologyTermService.Create(models);
            await _ontologyTermService.Update(updates);
            return Ok();
        }
    }
}
