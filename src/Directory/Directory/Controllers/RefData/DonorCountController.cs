using Common;
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
    public class DonorCountController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public DonorCountController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Donor Counts")]
        [SwaggerResponse(200, "All Donor Counts", typeof(List<DonorCountOutboundDto>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListDonorCounts());

        [SwaggerOperation("Get a single Donor Count by ID")]
        [SwaggerResponse(200, "The Donor Count with the requested ID.", typeof(DonorCountOutboundDto))]
        [SwaggerResponse(404, "No Donor Count was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var donorCount = await _readService.GetDonorCount(id);
            if (donorCount is null)
                return NotFound();

            return Ok(donorCount);
        }

        [SwaggerOperation("Creates a new Donor Count")]
        [SwaggerResponse(201, "The Donor Count was created", typeof(DonorCount))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DonorCountInboundDto donorCount)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdDonorCount = new DonorCountOutboundDto();

            try
            {
                createdDonorCount = await _writeService.CreateDonorCount(donorCount);
            }
            catch (KeyNotFoundException e)
            {
                if (e.Data.Contains(ExceptionData.KeyNotFound))
                {
                    return UnprocessableEntity(e.Data[ExceptionData.KeyNotFound]);
                }
            }

            return CreatedAtAction("Get", new { id = createdDonorCount.Id }, createdDonorCount);
        }

        [SwaggerOperation("Updates an existing Donor Count")]
        [SwaggerResponse(204, "The Donor Count was updated successfully.")]
        [SwaggerResponse(404, "No Donor Count was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] DonorCountInboundDto donorCount)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetDonorCount(id) is null)
                return NotFound();

            try
            {
                await _writeService.UpdateDonorCount(id, donorCount);
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

        [SwaggerOperation("Delete a single Donor Count by ID.")]
        [SwaggerResponse(204, "The Funder was succesfully deleted.")]
        [SwaggerResponse(404, "No Donor Count was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) 
            => await _writeService.DeleteDonorCount(id) ? NoContent() : (IActionResult)NotFound();
    }
}