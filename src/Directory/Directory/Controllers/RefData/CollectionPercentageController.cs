using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Directory.Controllers.RefData
{
    [AllowAnonymous]
    [Route("api/[controller]")]
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

        [SwaggerOperation(Description = "List of all Collection Percentages")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionPercentages());

        [SwaggerOperation(Description = "Get a single Collection Percentage by ID")]
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

        [SwaggerOperation(Description = "Creates a new a Collection Percentage")]
        [SwaggerResponse(201, "The Collection Percentage was created", typeof(CollectionPercentage))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPercentage)
        {
            var createdCollectionPercentage = await _writeService.CreateCollectionPercentage(collectionPercentage);
            return CreatedAtAction("Get", new { id = createdCollectionPercentage.Id }, createdCollectionPercentage);
        }

        [SwaggerOperation(Description = "Updates an existing Collection Percentage")]
        [SwaggerResponse(204, "The Collection Percentage was updated successfully.")]
        [SwaggerResponse(400, "No Collection Percentage was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPercentage)
        {
            if (_readService.GetCollectionPercentage(id) is null)
                return BadRequest();

            await _writeService.UpdateCollectionPercentage(id, collectionPercentage);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Collection Percentage by ID.")]
        [SwaggerResponse(204, "The Collection Percentage was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteCollectionPercentage(id);
            return NoContent();
        }
    }
}