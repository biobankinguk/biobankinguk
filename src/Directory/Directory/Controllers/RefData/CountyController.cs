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
    public class CountyController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CountyController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all County")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCounties());

        [SwaggerOperation("Get a single County by ID")]
        [SwaggerResponse(200, "The County with the requested ID.", typeof(County))]
        [SwaggerResponse(404, "No County was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var county = await _readService.GetCounty(id);
            if (county is null)
                return NotFound();

            return Ok(county);
        }

        [SwaggerOperation("Creates a new County")]
        [SwaggerResponse(201, "The County was created", typeof(Country))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto county)
        {
            var createdCounty = await _writeService.CreateCounty(county);
            return CreatedAtAction("Get", new { id = createdCounty.Id }, createdCounty);
        }

        [SwaggerOperation("Updates an existing County")]
        [SwaggerResponse(204, "The County was updated successfully.")]
        [SwaggerResponse(400, "No County was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto county)
        {
            if (_readService.GetCounty(id) is null)
                return BadRequest();

            await _writeService.UpdateCounty(id, county);

            return NoContent();
        }

        [SwaggerOperation("Delete a single County by ID.")]
        [SwaggerResponse(204, "The County was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteCounty(id);
            return NoContent();
        }
    }
}