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
    public class SexController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public SexController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListSexes());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var sex = await _readService.GetSex(id);
            if (sex is null)
                return NotFound();

            return Ok(sex);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto sex)
        {
            var createdSex = await _writeService.CreateSex(sex);
            return CreatedAtAction("Get", new { id = createdSex.Id }, createdSex);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto sex)
        {
            if (_readService.GetSex(id) is null)
                return BadRequest();

            await _writeService.UpdateSex(id, sex);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteSex(id);
            return NoContent();
        }
    }
}