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
    public class CountyController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CountyController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCounties());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var county = await _readService.GetCounty(id);
            if (county == null)
                return NotFound();

            return Ok(county);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto county)
        {
            var createdCounty = await _writeService.CreateCounty(county);
            return CreatedAtAction("Get", new { id = createdCounty.Id }, createdCounty);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto county)
        {
            if (_readService.GetCounty(id) == null)
                return BadRequest();

            await _writeService.UpdateCounty(id, county);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetCounty(id) == null)
                return NotFound();

            await _writeService.DeleteCounty(id);

            return NoContent();
        }
    }
}