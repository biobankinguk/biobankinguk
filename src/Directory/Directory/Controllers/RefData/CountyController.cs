using Common.Constants;
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
    public class CountyController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CountyController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Counties")]
        [SwaggerResponse(200, "All Counties", typeof(List<CountyOutboundDto>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCounties());

        [SwaggerOperation("Get a single County by ID")]
        [SwaggerResponse(200, "The County with the requested ID.", typeof(CountyOutboundDto))]
        [SwaggerResponse(404, "No County was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var county = await _readService.GetCounty(id);
            if (county is null)
                return NotFound();

            return Ok(county);
        }

        [SwaggerOperation("Creates a new County")]
        [SwaggerResponse(201, "The County was created", typeof(County))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CountyInboundDto county)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCounty = new County();

            try
            {
                createdCounty = await _writeService.CreateCounty(county);
            }
            catch (KeyNotFoundException e)
            {
                if (e.Data.Contains(ExceptionData.KeyNotFound))
                {
                    return UnprocessableEntity(e.Data[ExceptionData.KeyNotFound]);
                }
            }

            return CreatedAtAction("Get", new { id = createdCounty.Id }, createdCounty);
        }

        [SwaggerOperation("Updates an existing County")]
        [SwaggerResponse(204, "The County was updated successfully.")]
        [SwaggerResponse(404, "No County was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] CountyInboundDto county)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetCounty(id) is null)
                return NotFound();

            try
            {
                await _writeService.UpdateCounty(id, county);
            }
            catch (KeyNotFoundException e)
            {
                if (e.Data.Contains(ExceptionData.KeyNotFound))
                {
                    return UnprocessableEntity(e.Data[ExceptionData.KeyNotFound]);
                }
            }

            return NoContent();
        }

        [SwaggerOperation("Delete a single County by ID.")]
        [SwaggerResponse(204, "The County was succesfully deleted.")]
        [SwaggerResponse(404, "No County was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteCounty(id) ? (IActionResult) NoContent() : NotFound();
    }
}