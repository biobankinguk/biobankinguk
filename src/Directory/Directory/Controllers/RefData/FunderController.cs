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
    public class FunderController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public FunderController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListFunders());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var funder = await _readService.GetFunder(id);
            if (funder == null)
                return NotFound();

            return Ok(funder);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto funder)
        {
            var createdFunder = await _writeService.CreateFunder(funder);
            return CreatedAtAction("Get", new { id = createdFunder.Id }, createdFunder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto funder)
        {
            if (_readService.GetFunder(id) == null)
                return BadRequest();

            await _writeService.UpdateFunder(id, funder);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetFunder(id) == null)
                return NotFound();

            await _writeService.DeleteFunder(id);

            return NoContent();
        }
    }
}