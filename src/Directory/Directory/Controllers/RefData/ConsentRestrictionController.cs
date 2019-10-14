using Common.Data.ReferenceData;
using Common.DTO;
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
    public class ConsentRestrictionController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public ConsentRestrictionController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Consent Restrictions")]
        [SwaggerResponse(200, "All Consent Restrictions", typeof(List<ConsentRestriction>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListConsentRestrictions());

        [SwaggerOperation("Get a single Consent Restriction by ID")]
        [SwaggerResponse(200, "The Consent Restriction with the requested ID.", typeof(ConsentRestriction))]
        [SwaggerResponse(404, "No Consent Restriction was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var consentRestriction = await _readService.GetConsentRestriction(id);
            if (consentRestriction is null)
                return NotFound();

            return Ok(consentRestriction);
        }

        [SwaggerOperation("Creates a new Consent Restriction")]
        [SwaggerResponse(201, "The Consent Restriction was created", typeof(ConsentRestriction))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto consentRestriction)
        {
            var createdConsentRestriction = await _writeService.CreateConsentRestriction(consentRestriction);
            return CreatedAtAction("Get", new { id = createdConsentRestriction.Id }, createdConsentRestriction);
        }

        [SwaggerOperation("Updates an existing Consent Restriction")]
        [SwaggerResponse(204, "The Consent Restriction was updated successfully.")]
        [SwaggerResponse(404, "No Consent Restriction was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto consentRestriction)
        {
            if (_readService.GetConsentRestriction(id) is null)
                return NotFound();

            await _writeService.UpdateConsentRestriction(id, consentRestriction);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Consent Restriction by ID.")]
        [SwaggerResponse(204, "The Consent Restriction was succesfully deleted.")]
        [SwaggerResponse(404, "No Consent Restriction was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteConsentRestriction(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}