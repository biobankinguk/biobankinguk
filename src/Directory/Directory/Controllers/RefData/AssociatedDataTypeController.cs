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
    public class AssociatedDataTypeController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AssociatedDataTypeController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAssociatedDataTypes());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAssociatedDataType(id);
            if (collectionPoint == null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdAssociatedDataType = await _writeService.CreateAssociatedDataType(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAssociatedDataType.Id }, createdAssociatedDataType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetAssociatedDataType(id) == null)
                return BadRequest();

            await _writeService.UpdateAssociatedDataType(id, collectionPoint);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetAssociatedDataType(id) == null)
                return NotFound();

            await _writeService.DeleteAssociatedDataType(id);

            return NoContent();
        }
    }
}