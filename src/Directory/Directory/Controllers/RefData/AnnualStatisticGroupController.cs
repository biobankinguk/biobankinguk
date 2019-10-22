using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Directory.Controllers
{
    [AllowAnonymous]
    [Route("api/refdata/[controller]")]
    [ApiController]
    public class AnnualStatisticGroupController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AnnualStatisticGroupController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Annual Statistic Groups")]
        [SwaggerResponse(200, "All Annual Statistic Groups", typeof(List<AnnualStatisticGroup>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAnnualStatisticGroups());

        [SwaggerOperation("Get a single Annual Statistic Group by ID")]
        [SwaggerResponse(200, "The Annual Statistic Group with the requested ID.", typeof(AnnualStatistic))]
        [SwaggerResponse(404, "No Annual Statistic Group was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAnnualStatisticGroup(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Annual Statistic Group")]
        [SwaggerResponse(201, "The Annual Statistic Group was created", typeof(AnnualStatisticGroup))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto annualStatisticGroup)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdAnnualStatistic = await _writeService.CreateAnnualStatisticGroup(annualStatisticGroup);
            return CreatedAtAction("Get", new { id = createdAnnualStatistic.Id }, createdAnnualStatistic);
        }

        [SwaggerOperation("Updates an existing Annual Statistic Group")]
        [SwaggerResponse(204, "The Annual Statistic Group was updated successfully.")]
        [SwaggerResponse(404, "No Annual Statistic Group was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto annualStatisticGroup)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetAnnualStatisticGroup(id) is null)
                return NotFound();

            await _writeService.UpdateAnnualStatisticGroup(id, annualStatisticGroup);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Annual Statistic Group by ID.")]
        [SwaggerResponse(204, "The Annual Statistic Group was succesfully deleted.")]
        [SwaggerResponse(404, "No Annual Statistic Group was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteAnnualStatisticGroup(id) ? (IActionResult)NoContent() : NotFound();
    }
}


