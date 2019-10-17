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
    public class AnnualStatisticController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AnnualStatisticController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Annual Statistics")]
        [SwaggerResponse(200, "All Annual Statistics", typeof(List<AnnualStatisticDto>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAnnualStatistics());

        [SwaggerOperation("Get a single Annual Statistic by ID")]
        [SwaggerResponse(200, "The Annual Statistic with the requested ID.", typeof(AnnualStatisticDto))]
        [SwaggerResponse(404, "No Annual Statistic was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAnnualStatistic(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Annual Statistic")]
        [SwaggerResponse(201, "The Annual Statistic was created", typeof(AnnualStatistic))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AnnualStatisticDto annualStatistic)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdAnnualStatistic = await _writeService.CreateAnnualStatistic(annualStatistic);
            return CreatedAtAction("Get", new { id = createdAnnualStatistic.Id }, createdAnnualStatistic);
        }

        [SwaggerOperation("Updates an existing Annual Statistic")]
        [SwaggerResponse(204, "The Annual Statistic was updated successfully.")]
        [SwaggerResponse(404, "No Annual Statistic was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] AnnualStatisticDto annualStatistic)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetAnnualStatistic(id) is null)
                return NotFound();

            await _writeService.UpdateAnnualStatistic(id, annualStatistic);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Annual Statistic by ID.")]
        [SwaggerResponse(204, "The Annual Statistic was succesfully deleted.")]
        [SwaggerResponse(404, "No Annual Statistic was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteAnnualStatistic(id) ? (IActionResult) NoContent() : NotFound();
    }
}