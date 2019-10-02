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
    public class StorageTemperatureController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public StorageTemperatureController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListStorageTemperatures());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var storageTemperature = await _readService.GetStorageTemperature(id);
            if (storageTemperature == null)
                return NotFound();

            return Ok(storageTemperature);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto storageTemperature)
        {
            var createdStorageTemperature = await _writeService.CreateStorageTemperature(storageTemperature);
            return CreatedAtAction("Get", new { id = createdStorageTemperature.Id }, createdStorageTemperature);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto storageTemperature)
        {
            if (_readService.GetStorageTemperature(id) == null)
                return BadRequest();

            await _writeService.UpdateStorageTemperature(id, storageTemperature);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetStorageTemperature(id) == null)
                return NotFound();

            await _writeService.DeleteStorageTemperature(id);

            return NoContent();
        }
    }
}