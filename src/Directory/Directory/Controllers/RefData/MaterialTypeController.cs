﻿using Common.Data.ReferenceData;
using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        [SwaggerOperation("List of all Material Types")]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListMaterialTypes());

        [SwaggerOperation("Get a single Material Type by ID")]
        [SwaggerResponse(200, "The Material Type with the requested ID.", typeof(MaterialType))]
        [SwaggerResponse(404, "No Material Type was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var materialType = await _readService.GetMaterialType(id);
            if (materialType is null)
                return NotFound();

            return Ok(materialType);
        }

        [SwaggerOperation("Creates a new Material Type")]
        [SwaggerResponse(201, "The Material Type was created", typeof(MaterialType))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto materialType)
        {
            var createdMaterialType = await _writeService.CreateMaterialType(materialType);
            return CreatedAtAction("Get", new { id = createdMaterialType.Id }, createdMaterialType);
        }

        [SwaggerOperation("Updates an existing Material Type")]
        [SwaggerResponse(204, "The Material Type was updated successfully.")]
        [SwaggerResponse(400, "No Material Type was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto materialType)
        {
            if (_readService.GetMaterialType(id) is null)
                return BadRequest();

            await _writeService.UpdateMaterialType(id, materialType);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Material Type by ID.")]
        [SwaggerResponse(204, "The Material Type was succesfully deleted.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _writeService.DeleteMaterialType(id);
            return NoContent();
        }
    }
}