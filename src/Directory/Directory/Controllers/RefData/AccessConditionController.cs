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
    public class AccessConditionController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AccessConditionController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }
        [SwaggerOperation(Description = "List of all Access Conditions")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAccessConditions());

        [SwaggerOperation(Description = "Get a single Access Condition by ID")]
        [SwaggerResponse(200, "The Access Condition with the requested ID.", typeof(AccessCondition))]
        [SwaggerResponse(404, "No Access Condition was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAccessCondition(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation(Description ="Creates a new Access Condition")]
        [SwaggerResponse(201, "The Access Condition was created", typeof(AccessCondition))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdAccessCondition = await _writeService.CreateAccessCondition(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAccessCondition.Id }, createdAccessCondition);
        }

        [SwaggerOperation(Description = "Updates an existing Access Condition")]
        [SwaggerResponse(204, "The Access Condition was updated successfully.")]
        [SwaggerResponse(400, "No Access Condition was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetAccessCondition(id) is null)
                return BadRequest();

            await _writeService.UpdateAccessCondition(id, collectionPoint);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Access Condition by ID.")]
        [SwaggerResponse(204, "The Access Condition was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteAccessCondition(id);
            return NoContent();
        }
    }
}