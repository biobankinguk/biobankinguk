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
    public class CountryController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CountryController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCountries());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var country = await _readService.GetCountry(id);
            if (country is null)
                return NotFound();

            return Ok(country);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto country)
        {
            var createdCountry = await _writeService.CreateCountry(country);
            return CreatedAtAction("Get", new { id = createdCountry.Id }, createdCountry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto country)
        {
            if (_readService.GetCountry(id) is null)
                return BadRequest();

            await _writeService.UpdateCountry(id, country);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetCountry(id) is null)
                return NotFound();

            await _writeService.DeleteCountry(id);

            return NoContent();
        }
    }
}