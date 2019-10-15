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
    public class ServiceOfferingController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public ServiceOfferingController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Service Offerings")]
        [SwaggerResponse(200, "All Service Offerings", typeof(List<ServiceOffering>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListServiceOfferings());

        [SwaggerOperation("Get a single Service Offering by ID")]
        [SwaggerResponse(200, "The Service Offering with the requested ID.", typeof(ServiceOffering))]
        [SwaggerResponse(404, "No Service Offering was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var serviceOffering = await _readService.GetServiceOffering(id);
            if (serviceOffering is null)
                return NotFound();

            return Ok(serviceOffering);
        }

        [SwaggerOperation("Creates a new Service Offering")]
        [SwaggerResponse(201, "The Service Offering was created", typeof(ServiceOffering))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto serviceOffering)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdServiceOffering = await _writeService.CreateServiceOffering(serviceOffering);
            return CreatedAtAction("Get", new { id = createdServiceOffering.Id }, createdServiceOffering);
        }

        [SwaggerOperation("Updates an existing Service Offering")]
        [SwaggerResponse(204, "The Service Offering was updated successfully.")]
        [SwaggerResponse(404, "No Service Offering was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto serviceOffering)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetServiceOffering(id) is null)
                return NotFound();

            await _writeService.UpdateServiceOffering(id, serviceOffering);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Service Offering by ID.")]
        [SwaggerResponse(204, "The Service Offering was succesfully deleted.")]
        [SwaggerResponse(404, "No Service Offering was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteServiceOffering(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}