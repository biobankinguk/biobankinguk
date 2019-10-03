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
    public class CollectionPointController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionPointController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Collection Points")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionPoints());

        [SwaggerOperation("Get a single Collection Point by ID")]
        [SwaggerResponse(200, "The Collection Point with the requested ID.", typeof(CollectionPoint))]
        [SwaggerResponse(404, "No Collection Point was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetCollectionPoint(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Collection Point")]
        [SwaggerResponse(201, "The Collection Point was created", typeof(CollectionPoint))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdCollectionPoint = await _writeService.CreateCollectionPoint(collectionPoint);
            return CreatedAtAction("Get", new { id = createdCollectionPoint.Id }, createdCollectionPoint);
        }

        [SwaggerOperation("Updates an existing Collection Point")]
        [SwaggerResponse(204, "The Collection Point was updated successfully.")]
        [SwaggerResponse(400, "No Collection Point was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetCollectionPoint(id) is null)
                return BadRequest();

            await _writeService.UpdateCollectionPoint(id, collectionPoint);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Collection Point by ID.")]
        [SwaggerResponse(204, "The Collection Point was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteCollectionPoint(id);
            return NoContent();
        }
    }
}