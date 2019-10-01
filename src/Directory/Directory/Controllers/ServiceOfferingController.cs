using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Directory.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
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

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListServiceOfferings());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var serviceOffering = await _readService.GetServiceOffering(id);
            if (serviceOffering == null)
                return NotFound();

            return Ok(serviceOffering);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto serviceOffering)
        {
            var createdServiceOffering = await _writeService.CreateServiceOffering(serviceOffering);
            return CreatedAtAction("Get", new { id = createdServiceOffering.Id }, createdServiceOffering);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, SortedRefDataBaseDto serviceOffering)
        {
            if (_readService.GetServiceOffering(id) == null)
                return BadRequest();

            await _writeService.UpdateServiceOffering(id, serviceOffering);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetServiceOffering(id) == null)
                return NotFound();

            await _writeService.DeleteServiceOffering(id);

            return NoContent();
        }
    }
}