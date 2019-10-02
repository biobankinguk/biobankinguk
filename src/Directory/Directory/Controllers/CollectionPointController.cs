﻿using Common.DTO;
using Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Directory.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class CollectionPointController : Controller
    {
        private readonly IReferenceDataReadService _readService;
        private readonly IReferenceDataWriterService _writeService;

        public CollectionPointController(IReferenceDataReadService readService, IReferenceDataWriterService writeService)
        {
            _readService = readService;
            _writeService = writeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
           => Ok(await _readService.ListCollectionPoints());


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var collectionPoint = await _readService.GetCollectionPoint(id);
            if (collectionPoint == null)
                return NotFound();

            return Ok(collectionPoint);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SortedRefDataBaseDto collectionPoint)
        {
            var createdCollectionPoint = await _writeService.CreateCollectionPoint(collectionPoint);
            return CreatedAtAction("Get", new { id = createdCollectionPoint.Id }, createdCollectionPoint);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] SortedRefDataBaseDto collectionPoint)
        {
            if (_readService.GetCollectionPoint(id) == null)
                return BadRequest();

            await _writeService.UpdateCollectionPoint(id, collectionPoint);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _readService.GetCollectionPoint(id) == null)
                return NotFound();

            await _writeService.DeleteCollectionPoint(id);

            return NoContent();
        }
    }
}