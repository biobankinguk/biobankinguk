using Common.Data.ReferenceData;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Directory.Controllers.RefData
{
    [AllowAnonymous]
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListOntologyTerms());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var consentRestriction = await _readService.GetOntologyTerm(id);
            if (consentRestriction is null)
                return NotFound();

            return Ok(consentRestriction);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OntologyTerm ontologyTerm)
        {
            var createdOntologyTerm = await _writeService.CreateOntologyTerm(ontologyTerm);
            return CreatedAtAction("Get", new { id = createdOntologyTerm.Id }, createdOntologyTerm);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] OntologyTerm ontologyTerm)
        {
            if (_readService.GetOntologyTerm(id) is null)
                return BadRequest();

            await _writeService.UpdateOntologyTerm(id, ontologyTerm);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetConsentRestriction(id) is null)
                return NotFound();

            await _writeService.DeleteConsentRestriction(id);

            return NoContent();
        }
    }
}