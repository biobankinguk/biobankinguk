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
    public class MaterialTypeGroupController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public MaterialTypeGroupController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Material Type Groups")]
        [SwaggerResponse(200, "All Material Type Groups", typeof(List<MaterialTypeGroup>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListMaterialTypeGroups());

        [SwaggerOperation("Get a single Material Type Group by ID")]
        [SwaggerResponse(200, "The Material Type Group with the requested ID.", typeof(MaterialTypeGroup))]
        [SwaggerResponse(404, "No Material Type Group was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var materialTypeGroup = await _readService.GetMaterialTypeGroup(id);
            if (materialTypeGroup is null)
                return NotFound();

            return Ok(materialTypeGroup);
        }

        [SwaggerOperation("Creates a new Material Type Group")]
        [SwaggerResponse(201, "The Material Type Group was created", typeof(MaterialTypeGroup))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RefDataBaseDto materialTypeGroup)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdMaterialTypeGroup = await _writeService.CreateMaterialTypeGroup(materialTypeGroup);
            return CreatedAtAction("Get", new { id = createdMaterialTypeGroup.Id }, createdMaterialTypeGroup);
        }

        [SwaggerOperation("Updates an existing Material Type Group")]
        [SwaggerResponse(204, "The Material Type Group was updated successfully.")]
        [SwaggerResponse(404, "No Material Type Group was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] RefDataBaseDto materialTypeGroup)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetMaterialTypeGroup(id) is null)
                return NotFound();

            await _writeService.UpdateMaterialTypeGroup(id, materialTypeGroup);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Material Type Group by ID.")]
        [SwaggerResponse(204, "The Material Type Group was succesfully deleted.")]
        [SwaggerResponse(404, "No Material Type Group was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteMaterialTypeGroup(id) ? (IActionResult)NoContent() : NotFound();
    }
}