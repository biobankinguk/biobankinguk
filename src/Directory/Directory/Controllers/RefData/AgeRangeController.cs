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
    public class AgeRangeController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AgeRangeController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Age Ranges")]
        [SwaggerResponse(200, "All Age Ranges", typeof(List<AgeRange>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAgeRanges());

        [SwaggerOperation("Get a single Age Range by ID")]
        [SwaggerResponse(200, "The Age Range with the requested ID.", typeof(AgeRange))]
        [SwaggerResponse(404, "No Age Range was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAgeRange(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Age Range")]
        [SwaggerResponse(201, "The Age Range was created", typeof(AgeRange))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdAgeRange = await _writeService.CreateAgeRange(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAgeRange.Id }, createdAgeRange);
        }

        [SwaggerOperation("Updates an existing Age Range")]
        [SwaggerResponse(204, "The Age Range was updated successfully.")]
        [SwaggerResponse(404, "No Age Range was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetAgeRange(id) is null)
                return NotFound();

            await _writeService.UpdateAgeRange(id, collectionPoint);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Age Range by ID.")]
        [SwaggerResponse(204, "The Age Range was succesfully deleted.")]
        [SwaggerResponse(404, "No Age Range was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(await _writeService.DeleteAgeRange(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}