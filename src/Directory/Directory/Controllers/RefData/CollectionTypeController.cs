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
    public class CollectionTypeController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionTypeController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation(Description = "List of all Collection Types")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionTypes());

        [SwaggerOperation(Description = "Get a single Collection Type by ID")]
        [SwaggerResponse(200, "The Collection Type with the requested ID.", typeof(CollectionType))]
        [SwaggerResponse(404, "No Collection Type was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionType = await _readService.GetCollectionType(id);
            if (collectionType is null)
                return NotFound();

            return Ok(collectionType);
        }

        [SwaggerOperation(Description = "Creates a new Collection Type")]
        [SwaggerResponse(201, "The Collection Type was created", typeof(CollectionType))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionType)
        {
            var createdCollectionType = await _writeService.CreateCollectionType(collectionType);
            return CreatedAtAction("Get", new { id = createdCollectionType.Id }, createdCollectionType);
        }

        [SwaggerOperation(Description = "Updates an existing Collection Type")]
        [SwaggerResponse(204, "The Collection Type was updated successfully.")]
        [SwaggerResponse(400, "No Collection Type was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionType)
        {
            if (_readService.GetCollectionType(id) is null)
                return BadRequest();

            await _writeService.UpdateCollectionType(id, collectionType);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Collection Type by ID.")]
        [SwaggerResponse(204, "The Collection Type was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteCollectionType(id);
            return NoContent();
        }
    }
}