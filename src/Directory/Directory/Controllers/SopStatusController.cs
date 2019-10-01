using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Directory.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SopStatusController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public SopStatusController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
            => Ok(await _readService.ListSopStatuses());
        

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var sopStatus = await _readService.GetSopStatus(id);
            if (sopStatus == null)
                return NotFound();

            return Ok(sopStatus);
        }
            

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto sopStatus)
        {
            var createdSopStatus = await _writeService.CreateSopStatus(sopStatus);
            return CreatedAtAction("Get", new { id = createdSopStatus.Id }, createdSopStatus);
        }
         
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto sopStatus)
        {
            if (_readService.GetSopStatus(id) == null)
                return BadRequest();

            await _writeService.UpdateSopStatus(id, sopStatus);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetSopStatus(id) == null)
                return NotFound();

            await _writeService.DeleteSopStatus(id);

            return NoContent();
        }
    }
}
