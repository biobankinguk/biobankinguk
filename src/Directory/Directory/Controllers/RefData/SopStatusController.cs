using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Controllers.RefData
{
    [AllowAnonymous]
    [Route("api/refdata/[controller]")]
    [ApiController]
    public class SopStatusController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public SopStatusController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all SOP Statuses")]
        [SwaggerResponse(200, "All SOP Statuses", typeof(List<SopStatus>))]
        [HttpGet]
        public async Task<IActionResult> Index()
            => Ok(await _readService.ListSopStatuses());

        [SwaggerOperation("Get a single SOP Status by ID")]
        [SwaggerResponse(200, "The SOP Status with the requested ID.", typeof(SopStatus))]
        [SwaggerResponse(404, "No SOP Status was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var sopStatus = await _readService.GetSopStatus(id);
            if (sopStatus is null)
                return NotFound();

            return Ok(sopStatus);
        }

        [SwaggerOperation("Creates a new Material Type")]
        [SwaggerResponse(201, "The Material Type was created", typeof(SopStatus))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto sopStatus)
        {
            var createdSopStatus = await _writeService.CreateSopStatus(sopStatus);
            return CreatedAtAction("Get", new { id = createdSopStatus.Id }, createdSopStatus);
        }

        [SwaggerOperation("Updates an existing SOP Status")]
        [SwaggerResponse(204, "The SOP Status was updated successfully.")]
        [SwaggerResponse(404, "No SOP Status was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto sopStatus)
        {
            if (_readService.GetSopStatus(id) is null)
                return NotFound();

            await _writeService.UpdateSopStatus(id, sopStatus);

            return NoContent();
        }

        [SwaggerOperation("Delete a single SOP Status by ID.")]
        [SwaggerResponse(204, "The SOP Status was succesfully deleted.")]
        [SwaggerResponse(404, "No SOP Status was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteSopStatus(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}
