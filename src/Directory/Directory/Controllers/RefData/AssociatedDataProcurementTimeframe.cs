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
    public class AssociatedDataProcurementTimeframeController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AssociatedDataProcurementTimeframeController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAssociatedDataProcurementTimeframes());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAssociatedDataProcurementTimeframe(id);
            if (collectionPoint == null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdAssociatedDataProcurementTimeframe = await _writeService.CreateAssociatedDataProcurementTimeframe(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAssociatedDataProcurementTimeframe.Id }, createdAssociatedDataProcurementTimeframe);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetAssociatedDataProcurementTimeframe(id) == null)
                return BadRequest();

            await _writeService.UpdateAssociatedDataProcurementTimeframe(id, collectionPoint);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetAssociatedDataProcurementTimeframe(id) == null)
                return NotFound();

            await _writeService.DeleteAssociatedDataProcurementTimeframe(id);

            return NoContent();
        }
    }
}