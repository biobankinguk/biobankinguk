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
    public class DonorCountController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public DonorCountController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Donor Counts")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListDonorCounts());

        [SwaggerOperation("Get a single Donor Count by ID")]
        [SwaggerResponse(200, "The Donor Count with the requested ID.", typeof(County))]
        [SwaggerResponse(404, "No Donor Count was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var donorCount = await _readService.GetDonorCount(id);
            if (donorCount is null)
                return NotFound();

            return Ok(donorCount);
        }

        [SwaggerOperation("Creates a new Donor Count")]
        [SwaggerResponse(201, "The Donor Count was created", typeof(DonorCount))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto donorCount)
        {
            var createdDonorCount = await _writeService.CreateDonorCount(donorCount);
            return CreatedAtAction("Get", new { id = createdDonorCount.Id }, createdDonorCount);
        }

        [SwaggerOperation("Updates an existing Donor Count")]
        [SwaggerResponse(204, "The Donor Count was updated successfully.")]
        [SwaggerResponse(400, "No Donor Count was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto donorCount)
        {
            if (_readService.GetDonorCount(id) is null)
                return BadRequest();

            await _writeService.UpdateDonorCount(id, donorCount);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Donor Count by ID.")]
        [SwaggerResponse(204, "The County was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteDonorCount(id);
            return NoContent();
        }
    }
}