using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Directory.Controllers.RefData
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MacroscopicAssessmentController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public MacroscopicAssessmentController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Macroscopic Assessments")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListMacroscopicAssessments());

        [SwaggerOperation("Get a single Macroscopic Assessment by ID")]
        [SwaggerResponse(200, "The Macroscopic Assessment with the requested ID.", typeof(MacroscopicAssessment))]
        [SwaggerResponse(404, "No Macroscopic Assessment was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var macroscopicAssessment = await _readService.GetMacroscopicAssessment(id);
            if (macroscopicAssessment is null)
                return NotFound();

            return Ok(macroscopicAssessment);
        }

        [SwaggerOperation("Creates a new Macroscopic Assessment")]
        [SwaggerResponse(201, "The Macroscopic Assessment was created", typeof(MacroscopicAssessment))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto macroscopicAssessment)
        {
            var createdMacroscopicAssessment = await _writeService.CreateMacroscopicAssessment(macroscopicAssessment);
            return CreatedAtAction("Get", new { id = createdMacroscopicAssessment.Id }, createdMacroscopicAssessment);
        }

        [SwaggerOperation("Updates an existing Macroscopic Assessment")]
        [SwaggerResponse(204, "The Macroscopic Assessment was updated successfully.")]
        [SwaggerResponse(400, "No Macroscopic Assessment was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto macroscopicAssessment)
        {
            if (_readService.GetMacroscopicAssessment(id) is null)
                return BadRequest();

            await _writeService.UpdateMacroscopicAssessment(id, macroscopicAssessment);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Macroscopic Assessment by ID.")]
        [SwaggerResponse(204, "The Macroscopic Assessment was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteMacroscopicAssessment(id);
            return NoContent();
        }
    }
}