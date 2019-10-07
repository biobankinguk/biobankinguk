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
    public class StorageTemperatureController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public StorageTemperatureController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Storage Temperatures")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListStorageTemperatures());

        [SwaggerOperation("Get a single Storage Temperature by ID")]
        [SwaggerResponse(200, "The Storage Temperature with the requested ID.", typeof(StorageTemperature))]
        [SwaggerResponse(404, "No Storage Temperature was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var storageTemperature = await _readService.GetStorageTemperature(id);
            if (storageTemperature is null)
                return NotFound();

            return Ok(storageTemperature);
        }

        [SwaggerOperation("Creates a new Storage Temperature")]
        [SwaggerResponse(201, "The Storage Temperature was created", typeof(StorageTemperature))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto storageTemperature)
        {
            var createdStorageTemperature = await _writeService.CreateStorageTemperature(storageTemperature);
            return CreatedAtAction("Get", new { id = createdStorageTemperature.Id }, createdStorageTemperature);
        }

        [SwaggerOperation("Updates an existing Storage Temperature")]
        [SwaggerResponse(204, "The Storage Temperature was updated successfully.")]
        [SwaggerResponse(400, "No Storage Temperature was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto storageTemperature)
        {
            if (_readService.GetStorageTemperature(id) is null)
                return BadRequest();

            await _writeService.UpdateStorageTemperature(id, storageTemperature);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Storage Temperature by ID.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteStorageTemperature(id);
            return NoContent();
        }
    }
}