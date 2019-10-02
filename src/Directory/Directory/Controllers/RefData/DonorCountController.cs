using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListDonorCounts());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var donorCount = await _readService.GetDonorCount(id);
            if (donorCount == null)
                return NotFound();

            return Ok(donorCount);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto donorCount)
        {
            var createdDonorCount = await _writeService.CreateDonorCount(donorCount);
            return CreatedAtAction("Get", new { id = createdDonorCount.Id }, createdDonorCount);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto donorCount)
        {
            if (_readService.GetDonorCount(id) == null)
                return BadRequest();

            await _writeService.UpdateDonorCount(id, donorCount);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetDonorCount(id) == null)
                return NotFound();

            await _writeService.DeleteDonorCount(id);

            return NoContent();
        }
    }
}