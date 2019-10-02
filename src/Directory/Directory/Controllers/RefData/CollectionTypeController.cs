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
    public class CollectionTypeController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionTypeController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionTypes());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionType = await _readService.GetCollectionType(id);
            if (collectionType is null)
                return NotFound();

            return Ok(collectionType);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionType)
        {
            var createdCollectionType = await _writeService.CreateCollectionType(collectionType);
            return CreatedAtAction("Get", new { id = createdCollectionType.Id }, createdCollectionType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionType)
        {
            if (_readService.GetCollectionType(id) is null)
                return BadRequest();

            await _writeService.UpdateCollectionType(id, collectionType);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetCollectionType(id) is null)
                return NotFound();

            await _writeService.DeleteCollectionType(id);

            return NoContent();
        }
    }
}