using Common.Data.ReferenceData;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Controllers.RefData
{
    [AllowAnonymous]
    [Route("api/refdata/[controller]")]
    [ApiController]
    public class OntologyTermController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public OntologyTermController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Ontology Terms")]
        [SwaggerResponse(200, "All Ontology Terms", typeof(List<OntologyTerm>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListOntologyTerms());

        [SwaggerOperation("Get a single Ontology Term by ID")]
        [SwaggerResponse(200, "The Ontology Term with the requested ID.", typeof(OntologyTerm))]
        [SwaggerResponse(404, "No Ontology Term was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var consentRestriction = await _readService.GetOntologyTerm(id);
            if (consentRestriction is null)
                return NotFound();

            return Ok(consentRestriction);
        }

        [SwaggerOperation("Creates a new Ontology Term")]
        [SwaggerResponse(201, "The Ontology Term was created", typeof(OntologyTerm))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OntologyTerm ontologyTerm)
        {
            var createdOntologyTerm = await _writeService.CreateOntologyTerm(ontologyTerm);
            return CreatedAtAction("Get", new { id = createdOntologyTerm.Id }, createdOntologyTerm);
        }

        [SwaggerOperation("Updates an existing Ontology Term")]
        [SwaggerResponse(204, "The Ontology Term was updated successfully.")]
        [SwaggerResponse(404, "No Ontology Term was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] OntologyTerm ontologyTerm)
        {
            if (_readService.GetOntologyTerm(id) is null)
                return NotFound();

            await _writeService.UpdateOntologyTerm(id, ontologyTerm);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Ontology Term by ID.")]
        [SwaggerResponse(204, "The Ontology Term was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteConsentRestriction(id);
            return NoContent();
        }
    }
}