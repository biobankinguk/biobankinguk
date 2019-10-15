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
    public class CollectionStatusController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionStatusController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Collection Statuses")]
        [SwaggerResponse(200, "All Collection Statuses", typeof(List<CollectionStatus>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionStatuses());

        [SwaggerOperation("Get a single Collection Status by ID")]
        [SwaggerResponse(200, "The Collection Status with the requested ID.", typeof(CollectionStatus))]
        [SwaggerResponse(404, "No Collection Status was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionStatus = await _readService.GetCollectionStatus(id);
            if (collectionStatus is null)
                return NotFound();

            return Ok(collectionStatus);
        }

        [SwaggerOperation("Creates a new Collection Status")]
        [SwaggerResponse(201, "The Collection Status was created", typeof(CollectionStatus))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionStatus)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCollectionStatus = await _writeService.CreateCollectionStatus(collectionStatus);
            return CreatedAtAction("Get", new { id = createdCollectionStatus.Id }, createdCollectionStatus);
        }

        [SwaggerOperation("Updates an existing Collection Status")]
        [SwaggerResponse(204, "The Collection Status was updated successfully.")]
        [SwaggerResponse(404, "No Collection Status was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionStatus)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetCollectionStatus(id) is null)
                return NotFound();

            await _writeService.UpdateCollectionStatus(id, collectionStatus);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Collection Status by ID.")]
        [SwaggerResponse(204, "The Collection Status was succesfully deleted.")]
        [SwaggerResponse(404, "No Collection Status was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteCollectionStatus(id) ? (IActionResult) NoContent() : NotFound();
        
    }
}