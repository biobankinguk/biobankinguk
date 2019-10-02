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
    public class MaterialTypeController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public MaterialTypeController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListMaterialTypes());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var materialType = await _readService.GetMaterialType(id);
            if (materialType == null)
                return NotFound();

            return Ok(materialType);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto materialType)
        {
            var createdMaterialType = await _writeService.CreateMaterialType(materialType);
            return CreatedAtAction("Get", new { id = createdMaterialType.Id }, createdMaterialType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto materialType)
        {
            if (_readService.GetMaterialType(id) == null)
                return BadRequest();

            await _writeService.UpdateMaterialType(id, materialType);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetMaterialType(id) == null)
                return NotFound();

            await _writeService.DeleteMaterialType(id);

            return NoContent();
        }
    }
}