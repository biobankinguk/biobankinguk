using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Controllers.RefData
{
    [AllowAnonymous]
    [Route("api/refdata/[controller]")]
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

        [SwaggerOperation("List of all Countries")]
        [SwaggerResponse(200, "All Countries", typeof(List<Country>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCountries());

        [SwaggerOperation("Get a single Country by ID")]
        [SwaggerResponse(200, "The Country with the requested ID.", typeof(Country))]
        [SwaggerResponse(404, "No Country was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var country = await _readService.GetCountry(id);
            if (country is null)
                return NotFound();

            return Ok(country);
        }

        [SwaggerOperation("Creates a new Country")]
        [SwaggerResponse(201, "The Country was created", typeof(Country))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto country)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCountry = await _writeService.CreateCountry(country);
            return CreatedAtAction("Get", new { id = createdCountry.Id }, createdCountry);
        }

        [SwaggerOperation("Updates an existing Country")]
        [SwaggerResponse(204, "The Country was updated successfully.")]
        [SwaggerResponse(404, "No Country was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto country)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetCountry(id) is null)
                return NotFound();

            await _writeService.UpdateCountry(id, country);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Country by ID.")]
        [SwaggerResponse(204, "The Country was succesfully deleted.")]
        [SwaggerResponse(404, "No Country was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteCountry(id) ? (IActionResult) NoContent() : NotFound();
    }
}