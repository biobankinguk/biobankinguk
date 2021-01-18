using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Common.Auth;
using Biobanks.SubmissionApi.Models;
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
    public class OntologyController : ControllerBase
    {
        private readonly IOntologyService _ontologyService;

        private readonly IMapper _mapper;

        /// <inheritdoc />
        public OntologyController(IOntologyService ontologyService, IMapper mapper)
        {
            _ontologyService = ontologyService;
            _mapper = mapper;
        }

        /// <summary>
        /// Lists all the available Ontology entities.
        /// </summary>
        /// <returns>A list of Ontology entities.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<OntologyModel>))]
        public async Task<IActionResult> Get()
            => Ok(_mapper.Map<ICollection<OntologyModel>>(await _ontologyService.List()));

        /// <summary>
        /// Gets a specific Ontology entity.
        /// </summary>
        /// <param name="id">The ID of the Ontology entity to return.</param>
        /// <returns>A single Ontology entity.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(OntologyModel))]
        [SwaggerResponse(404, "Ontology with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var ontology = await _ontologyService.Get(id);
            if (ontology != null)
                return Ok(_mapper.Map<OntologyModel>(ontology));

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Ontology entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Ontology to add or update.</param>
        /// <param name="model">Ontology object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] Ontology model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _ontologyService.Get(id) == null)
            {
                await _ontologyService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _ontologyService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds an Ontology entity with an auto-generated identity.
        /// </summary>
        /// <param name="value">Ontology name value to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            //check we're maintaining unique values
            if (await _ontologyService.GetByValue(value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, value);

            var ontology = await _ontologyService.Create(new Ontology{Value = value});

            return CreatedAtAction("Get", new { id = ontology }, ontology);
        }
    }
}
