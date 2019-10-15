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
    public class SexController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public SexController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Sexes")]
        [SwaggerResponse(200, "All Sexes", typeof(List<Sex>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListSexes());

        [SwaggerOperation("Get a single Sex by ID")]
        [SwaggerResponse(200, "The Sex with the requested ID.", typeof(Sex))]
        [SwaggerResponse(404, "No Sex was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var sex = await _readService.GetSex(id);
            if (sex is null)
                return NotFound();

            return Ok(sex);
        }

        [SwaggerOperation("Creates a new Sex")]
        [SwaggerResponse(201, "The Sex was created", typeof(Sex))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto sex)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSex = await _writeService.CreateSex(sex);
            return CreatedAtAction("Get", new { id = createdSex.Id }, createdSex);
        }

        [SwaggerOperation("Updates an existing Sex")]
        [SwaggerResponse(204, "The Sex was updated successfully.")]
        [SwaggerResponse(404, "No Sex was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto sex)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetSex(id) is null)
                return NotFound();

            await _writeService.UpdateSex(id, sex);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Sex by ID.")]
        [SwaggerResponse(204, "The Sex was deleted successfully.")]
        [SwaggerResponse(404, "No Sex was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteSex(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}