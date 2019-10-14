using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Controllers
{
    [AllowAnonymous]
    [Route("api/refdata/[controller]")]
    [ApiController]
    public class HtaStatusController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public HtaStatusController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all HTA Statuses")]
        [SwaggerResponse(200, "All HTA Statuses", typeof(List<HtaStatus>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListHtaStatuses());

        [SwaggerOperation("Get a single HtaStatus by ID")]
        [SwaggerResponse(200, "The HtaStatus with the requested ID.", typeof(HtaStatus))]
        [SwaggerResponse(404, "No HtaStatus was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetHtaStatus(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Hta Status")]
        [SwaggerResponse(201, "The HtaStatus was created", typeof(HtaStatus))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdHtaStatus = await _writeService.CreateHtaStatus(collectionPoint);
            return CreatedAtAction("Get", new { id = createdHtaStatus.Id }, createdHtaStatus);
        }

        [SwaggerOperation("Updates an existing HtaStatus")]
        [SwaggerResponse(204, "The HtaStatus was updated successfully.")]
        [SwaggerResponse(404, "No HtaStatus was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetHtaStatus(id) is null)
                return NotFound();

            await _writeService.UpdateHtaStatus(id, collectionPoint);

            return NoContent();
        }

        [SwaggerOperation("Delete a single HtaStatus by ID.")]
        [SwaggerResponse(204, "The HtaStatus was succesfully deleted.")]
        [SwaggerResponse(404, "No HtaStatus was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteHtaStatus(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}