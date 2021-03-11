using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Submissions.Api.Models;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Biobanks.Entities.Api.ReferenceData;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class OntologyVersionController : ControllerBase
    {
        private readonly IOntologyVersionService _ontologyVersionService;

        private readonly IMapper _mapper;

        /// <inheritdoc />
        public OntologyVersionController(IOntologyVersionService ontologyVersionService, IMapper mapper)
        {
            _ontologyVersionService = ontologyVersionService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lists all the available Ontology Version entities.
        /// </summary>
        /// <returns>A list of Ontology Version entities.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<OntologyVersionModel>))]
        public async Task<IActionResult> Get() => Ok(_mapper.Map<ICollection<OntologyVersionModel>>(await _ontologyVersionService.List(null)));

        /// <summary>
        /// Gets a specific Ontology entity.
        /// </summary>
        /// <param name="id">The ID of the Ontology entity to return.</param>
        /// <returns>A single Ontology entity.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(OntologyVersionModel))]
        [SwaggerResponse(404, "Ontology Version with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var ontologyVersion = await _ontologyVersionService.Get(id);
            if (ontologyVersion != null)
                return Ok(_mapper.Map<OntologyVersionModel>(ontologyVersion));

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Ontology Version entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Ontology Version to add or update.</param>
        /// <param name="model">Ontology Version object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] OntologyVersion model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _ontologyVersionService.Get(id) == null)
            {
                await _ontologyVersionService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _ontologyVersionService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds an Ontology Version entity with an auto-generated identity.
        /// </summary>
        /// <param name="model">Ontology Version object to add.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] OntologyVersion model)
        {
            //for string keys we need to check for pre-existence
            if (await _ontologyVersionService.GetByValue(model.Value) != null)
                return StatusCode((int) HttpStatusCode.Conflict, model);

            return CreatedAtAction(
                "Get",
                new {id = model.Value},
                await _ontologyVersionService.Create(model));
        }
    }
}
