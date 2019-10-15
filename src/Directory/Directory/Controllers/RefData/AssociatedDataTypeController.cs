﻿using Common.Data.ReferenceData;
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
    public class AssociatedDataTypeController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public AssociatedDataTypeController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [SwaggerOperation("List of all Associated Data Types")]
        [SwaggerResponse(200, "All Associated Data Types", typeof(List<AssociatedDataType>))]
        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListAssociatedDataTypes());

        [SwaggerOperation("Get a single Associated Data Type by ID")]
        [SwaggerResponse(200, "The Associated Data Type with the requested ID.", typeof(AssociatedDataType))]
        [SwaggerResponse(404, "No Associated Data Type was found with the provided ID.")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetAssociatedDataType(id);
            if (collectionPoint is null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [SwaggerOperation("Creates a new Associated Data Type")]
        [SwaggerResponse(201, "The Associated Data Type was created", typeof(AssociatedDataType))]
        [SwaggerResponse(400, "The data is invalid")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdAssociatedDataType = await _writeService.CreateAssociatedDataType(collectionPoint);
            return CreatedAtAction("Get", new { id = createdAssociatedDataType.Id }, createdAssociatedDataType);
        }

        [SwaggerOperation("Updates an existing Associated Data Type")]
        [SwaggerResponse(204, "The Associated Data Type was updated successfully.")]
        [SwaggerResponse(404, "No Associated Data Type was found with the provided ID.")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_readService.GetAssociatedDataType(id) is null)
                return NotFound();

            await _writeService.UpdateAssociatedDataType(id, collectionPoint);

            return NoContent();
        }

        [SwaggerOperation("Delete a single Associated Data Type by ID.")]
        [SwaggerResponse(204, "The Associated Data Type was succesfully deleted.")]
        [SwaggerResponse(404, "No Associated Data Type was found with the provided ID. It may have previously been deleted or not yet created.")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _writeService.DeleteAssociatedDataType(id))
                return NoContent();
            else
                return NotFound();
        }
    }
}