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
    public class MacroscopicAssessmentController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public MacroscopicAssessmentController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListMacroscopicAssessments());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var macroscopicAssessment = await _readService.GetMacroscopicAssessment(id);
            if (macroscopicAssessment is null)
                return NotFound();

            return Ok(macroscopicAssessment);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto macroscopicAssessment)
        {
            var createdMacroscopicAssessment = await _writeService.CreateMacroscopicAssessment(macroscopicAssessment);
            return CreatedAtAction("Get", new { id = createdMacroscopicAssessment.Id }, createdMacroscopicAssessment);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto macroscopicAssessment)
        {
            if (_readService.GetMacroscopicAssessment(id) is null)
                return BadRequest();

            await _writeService.UpdateMacroscopicAssessment(id, macroscopicAssessment);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetMacroscopicAssessment(id) is null)
                return NotFound();

            await _writeService.DeleteMacroscopicAssessment(id);

            return NoContent();
        }
    }
}