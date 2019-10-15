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
    public class CollectionPercentageController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionPercentageController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Collection Percentages")]
        [SwaggerResponse(200, "All Collection Percentages", typeof(List<CollectionPercentage>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionPercentages());

        [SwaggerOperation("Get a single Collection Percentage by ID")]
        [SwaggerResponse(200, "The Collection Percentage with the requested ID.", typeof(CollectionPercentage))]
        [SwaggerResponse(404, "No Collection Percentage was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPercentage = await _readService.GetCollectionPercentage(id);
            if (collectionPercentage is null)
                return NotFound();

            return Ok(collectionPercentage);
        }

        [SwaggerOperation("Creates a new a Collection Percentage")]
        [SwaggerResponse(201, "The Collection Percentage was created", typeof(CollectionPercentage))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPercentage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCollectionPercentage = await _writeService.CreateCollectionPercentage(collectionPercentage);
            return CreatedAtAction("Get", new { id = createdCollectionPercentage.Id }, createdCollectionPercentage);
        }

        [SwaggerOperation("Updates an existing Collection Percentage")]
        [SwaggerResponse(204, "The Collection Percentage was updated successfully.")]
        [SwaggerResponse(404, "No Collection Percentage was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPercentage)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetCollectionPercentage(id) is null)
                return NotFound();

            await _writeService.UpdateCollectionPercentage(id, collectionPercentage);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Collection Percentage by ID.")]
        [SwaggerResponse(204, "The Collection Percentage was succesfully deleted.")]
        [SwaggerResponse(404, "No Collection Percentage was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteCollectionPercentage(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}