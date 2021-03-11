using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Entities.Api.ReferenceData;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class SampleContentMethodController : ControllerBase
    {
        private readonly ISampleContentMethodService _sampleContentMethodService;

        /// <inheritdoc />
        public SampleContentMethodController(ISampleContentMethodService sampleContentMethodService)
        {
            _sampleContentMethodService = sampleContentMethodService;
        }

        /// <summary>
        /// Lists all the available Sample Content Methods.
        /// </summary>
        /// <returns>A list of Sample Content Methods.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<SampleContentMethod>))]
        public async Task<IActionResult> Get() => Ok(await _sampleContentMethodService.List());

        /// <summary>
        /// Gets a specific Sample Content Method.
        /// </summary>
        /// <param name="id">The ID of the Sample Content Method to return.</param>
        /// <returns>A single Sample Content Method.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(SampleContentMethod))]
        [SwaggerResponse(404, "Sample Content Method with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var sampleContentMethod = await _sampleContentMethodService.Get(id);
            if (sampleContentMethod != null)
                return Ok(sampleContentMethod);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Sample Content Method entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Sample Content Method to add or update.</param>
        /// <param name="model">Sample Content Method object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] SampleContentMethod model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _sampleContentMethodService.Get(id) == null)
            {
                await _sampleContentMethodService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _sampleContentMethodService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds or updates a Sample Content Method entity with an auto-generated identity.
        /// </summary>
        /// <param name="value">Sample Content Method name value to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            //check we're maintaining unique values
            if (await _sampleContentMethodService.GetByValue(value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, value);

            var sampleContentMethod = await _sampleContentMethodService.Create(new SampleContentMethod{Value = value});

            return CreatedAtAction("Get", new { id = sampleContentMethod.Id }, sampleContentMethod);
        }
    }
}
