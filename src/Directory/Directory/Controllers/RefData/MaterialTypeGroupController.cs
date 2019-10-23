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
        [SwaggerResponse(200, "All Material Type Groups", typeof(List<MaterialTypeGroupOutboundDto>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListMaterialTypeGroups());

        [SwaggerOperation("Get a single Material Type Group by ID")]
        [SwaggerResponse(200, "The Material Type Group with the requested ID.", typeof(MaterialTypeGroupOutboundDto))]
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
        public async Task<IActionResult> Post([FromBody] MaterialTypeGroupInboundDto materialTypeGroup)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (materialTypeGroup.MaterialTypeIds.Count == 0)
                return BadRequest("No values in MaterialTypeIds have been provided.");

            var createdMaterialTypeGroup = new MaterialTypeGroupOutboundDto();

            try
            {
                createdMaterialTypeGroup = await _writeService.CreateMaterialTypeGroup(materialTypeGroup);
            }
            catch (KeyNotFoundException e)
            {
                if (e.Data.Contains(ExceptionData.KeyNotFound))
                {
                    return UnprocessableEntity(e.Data[ExceptionData.KeyNotFound]);
                }
            }

            return CreatedAtAction("Get", new { id = createdMaterialTypeGroup.Id }, createdMaterialTypeGroup);
        }

        [SwaggerOperation("Updates an existing Material Type Group")]
        [SwaggerResponse(204, "The Material Type Group was updated successfully.")]
        [SwaggerResponse(404, "No Material Type Group was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] MaterialTypeGroupInboundDto materialTypeGroup)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            else if (materialTypeGroup.MaterialTypeIds.Count == 0)
                return BadRequest("No values in MaterialTypeIds have been provided.");

            if (_readService.GetMaterialTypeGroup(id) is null)
                return NotFound();

            try
            {
                await _writeService.UpdateMaterialTypeGroup(id, materialTypeGroup);
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

        [SwaggerOperation("Delete a single Material Type Group by ID.")]
        [SwaggerResponse(204, "The Material Type Group was succesfully deleted.")]
        [SwaggerResponse(404, "No Material Type Group was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _writeService.DeleteMaterialTypeGroup(id) ? (IActionResult)NoContent() : NotFound();
    }
}