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
    public class AccessConditionController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AccessConditionController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAccessConditions());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAccessCondition(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdAccessCondition = await _writeService.CreateAccessCondition(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAccessCondition.Id }, createdAccessCondition);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetAccessCondition(id) is null)
                return BadRequest();

            await _writeService.UpdateAccessCondition(id, collectionPoint);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteAccessCondition(id);
            return NoContent();
        }
    }
}