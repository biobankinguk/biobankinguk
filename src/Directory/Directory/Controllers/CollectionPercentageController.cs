using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Directory.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionPercentageController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionPercentageController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionPercentages());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPercentage = await _readService.GetCollectionPercentage(id);
            if (collectionPercentage == null)
                return NotFound();

            return Ok(collectionPercentage);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPercentage)
        {
            var createdCollectionPercentage = await _writeService.CreateCollectionPercentage(collectionPercentage);
            return CreatedAtAction("Get", new { id = createdCollectionPercentage.Id }, createdCollectionPercentage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPercentage)
        {
            if (_readService.GetCollectionPercentage(id) == null)
                return BadRequest();

            await _writeService.UpdateCollectionPercentage(id, collectionPercentage);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetCollectionPercentage(id) == null)
                return NotFound();

            await _writeService.DeleteCollectionPercentage(id);

            return NoContent();
        }
    }
}