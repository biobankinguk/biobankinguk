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
    public class AccessConditionController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AccessConditionController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Access Conditions")]
        [SwaggerResponse(200, "All Access Conditions", typeof(List<AccessCondition>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAccessConditions());

        [SwaggerOperation("Get a single Access Condition by ID")]
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

        [SwaggerOperation("Creates a new Access Condition")]
        [SwaggerResponse(201, "The Access Condition was created", typeof(AccessCondition))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdAccessCondition = await _writeService.CreateAccessCondition(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAccessCondition.Id }, createdAccessCondition);
        }

        [SwaggerOperation("Updates an existing Access Condition")]
        [SwaggerResponse(204, "The Access Condition was updated successfully.")]
        [SwaggerResponse(404, "No Access Condition was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetAccessCondition(id) is null)
                return NotFound();

            await _writeService.UpdateAccessCondition(id, collectionPoint);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Access Condition by ID.")]
        [SwaggerResponse(204, "The Access Condition was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteAccessCondition(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}