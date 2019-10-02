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
    public class CollectionStatusController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionStatusController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionStatuses());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionStatus = await _readService.GetCollectionStatus(id);
            if (collectionStatus is null)
                return NotFound();

            return Ok(collectionStatus);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionStatus)
        {
            var createdCollectionStatus = await _writeService.CreateCollectionStatus(collectionStatus);
            return CreatedAtAction("Get", new { id = createdCollectionStatus.Id }, createdCollectionStatus);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionStatus)
        {
            if (_readService.GetCollectionStatus(id) is null)
                return BadRequest();

            await _writeService.UpdateCollectionStatus(id, collectionStatus);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetCollectionStatus(id) is null)
                return NotFound();

            await _writeService.DeleteCollectionStatus(id);

            return NoContent();
        }
    }
}