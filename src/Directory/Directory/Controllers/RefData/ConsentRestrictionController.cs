using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Directory.Controllers.RefData
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ConsentRestrictionController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public ConsentRestrictionController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListConsentRestrictions());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var consentRestriction = await _readService.GetConsentRestriction(id);
            if (consentRestriction is null)
                return NotFound();

            return Ok(consentRestriction);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto consentRestriction)
        {
            var createdConsentRestriction = await _writeService.CreateConsentRestriction(consentRestriction);
            return CreatedAtAction("Get", new { id = createdConsentRestriction.Id }, createdConsentRestriction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto consentRestriction)
        {
            if (_readService.GetConsentRestriction(id) is null)
                return BadRequest();

            await _writeService.UpdateConsentRestriction(id, consentRestriction);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteConsentRestriction(id);
            return NoContent();
        }
    }
}