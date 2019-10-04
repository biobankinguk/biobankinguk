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
    public class FunderController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public FunderController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Funders")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListFunders());

        [SwaggerOperation("Get a single Funder by ID")]
        [SwaggerResponse(200, "The Funder with the requested ID.", typeof(Funder))]
        [SwaggerResponse(404, "No Funder was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var funder = await _readService.GetFunder(id);
            if (funder is null)
                return NotFound();

            return Ok(funder);
        }

        [SwaggerOperation("Creates a new Funder")]
        [SwaggerResponse(201, "The Funder was created", typeof(Funder))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto funder)
        {
            var createdFunder = await _writeService.CreateFunder(funder);
            return CreatedAtAction("Get", new { id = createdFunder.Id }, createdFunder);
        }

        [SwaggerOperation("Updates an existing Funder")]
        [SwaggerResponse(204, "The Funder was updated successfully.")]
        [SwaggerResponse(400, "No Funder was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto funder)
        {
            if (_readService.GetFunder(id) is null)
                return BadRequest();

            await _writeService.UpdateFunder(id, funder);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Funder by ID.")]
        [SwaggerResponse(204, "The Funder was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteFunder(id);
            return NoContent();
        }
    }
}