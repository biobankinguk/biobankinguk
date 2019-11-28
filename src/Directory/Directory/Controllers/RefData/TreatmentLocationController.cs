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
    public class TreatmentLocationController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public TreatmentLocationController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Treatment Locations")]
        [SwaggerResponse(200, "All Treatment Locations", typeof(List<TreatmentLocation>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListTreatmentLocations());

        [SwaggerOperation("Get a single Treatment Location by ID")]
        [SwaggerResponse(200, "The Treatment Location with the requested ID.", typeof(TreatmentLocation))]
        [SwaggerResponse(404, "No Treatment Location was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var treatmentLocation = await _readService.GetTreatmentLocation(id);
            if (treatmentLocation is null)
                return NotFound();

            return Ok(treatmentLocation);
        }

        [SwaggerOperation("Creates a new Treatement Location")]
        [SwaggerResponse(201, "The Treatment Location was created", typeof(TreatmentLocation))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto treatmentLocation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdTreatmentLocation = await _writeService.CreateTreatmentLocation(treatmentLocation);
            return CreatedAtAction("Get", new { id = createdTreatmentLocation.Id }, createdTreatmentLocation);
        }

        [SwaggerOperation("Updates an existing Treatement Location")]
        [SwaggerResponse(204, "The Treatment Location was updated successfully.")]
        [SwaggerResponse(404, "No Treatment Location was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto treatmentLocation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetTreatmentLocation(id) is null)
                return NotFound();

            await _writeService.UpdateTreatmentLocation(id, treatmentLocation);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Treatment Location by ID.")]
        [SwaggerResponse(204, "The Treatment Location was succesfully deleted.")]
        [SwaggerResponse(404, "No Treatment Location was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteTreatmentLocation(id) ? (ActionResult)NoContent() : NotFound();
    }
}