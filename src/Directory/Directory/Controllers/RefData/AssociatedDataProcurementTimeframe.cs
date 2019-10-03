using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        [SwaggerOperation("List of all Associated Data Procurement Timeframe")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAssociatedDataProcurementTimeframes());

        [SwaggerOperation("Get a single Associated Data Procurement Timeframe by ID")]
        [SwaggerResponse(200, "The Associated Data Procurement Timeframe with the requested ID.", typeof(AssociatedDataProcurementTimeframe))]
        [SwaggerResponse(404, "No Associated Data Procurement Timeframe was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAssociatedDataProcurementTimeframe(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Associated Data Procurement Timeframe")]
        [SwaggerResponse(201, "The Associated Data Procurement Timeframe was created", typeof(AssociatedDataProcurementTimeframe))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdAssociatedDataProcurementTimeframe = await _writeService.CreateAssociatedDataProcurementTimeframe(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAssociatedDataProcurementTimeframe.Id }, createdAssociatedDataProcurementTimeframe);
        }

        [SwaggerOperation("Updates an existing Associated Data Procurement Timeframe")]
        [SwaggerResponse(204, "The Associated Data Procurement Timeframe was updated successfully.")]
        [SwaggerResponse(400, "No Associated Data Procurement Timeframe was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetAssociatedDataProcurementTimeframe(id) is null)
                return BadRequest();

            await _writeService.UpdateAssociatedDataProcurementTimeframe(id, collectionPoint);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Associated Data Procurement Timeframe by ID.")]
        [SwaggerResponse(204, "The Associated Data Procurement Timeframe was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteAssociatedDataProcurementTimeframe(id);
            return NoContent();
        }
    }
}