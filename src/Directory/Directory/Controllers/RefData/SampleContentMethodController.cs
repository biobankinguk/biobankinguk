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
    public class SampleContentMethodController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public SampleContentMethodController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Sample Content Methods")]
        [SwaggerResponse(200, "All Sample Content Methods", typeof(List<SampleContentMethod>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListSampleContentMethods());

        [SwaggerOperation("Get a single Sample Content Method by ID")]
        [SwaggerResponse(200, "The Sample Content Method with the requested ID.", typeof(SampleContentMethod))]
        [SwaggerResponse(404, "No Sample Content Method was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetSampleContentMethod(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Sample Content Method")]
        [SwaggerResponse(201, "The Sample Content Method was created", typeof(SampleContentMethod))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto sampleContentMethod)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSampleContentMethod = await _writeService.CreateSampleContentMethod(sampleContentMethod);
            return CreatedAtAction("Get", new { id = createdSampleContentMethod.Id }, createdSampleContentMethod);
        }

        [SwaggerOperation("Updates an existing Sample Content Method")]
        [SwaggerResponse(204, "The Sample Content Method was updated successfully.")]
        [SwaggerResponse(404, "No Sample Content Method was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto sampleContentMethod)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetSampleContentMethod(id) is null)
                return NotFound();

            await _writeService.UpdateSampleContentMethod(id, sampleContentMethod);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Sample Content Method by ID.")]
        [SwaggerResponse(204, "The Sample Content Method was succesfully deleted.")]
        [SwaggerResponse(404, "No Sample Content Method was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteSampleContentMethod(id) ? (ActionResult)NoContent() : NotFound();
    }
}