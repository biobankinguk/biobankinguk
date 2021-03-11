using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Api.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class SexController : ControllerBase
    {
        private readonly ISexService _sexService;

        /// <inheritdoc />
        public SexController(ISexService sexService)
        {
            _sexService = sexService;
        }

        /// <summary>
        /// Lists all the available Sex entities.
        /// </summary>
        /// <returns>A list of Sex entities.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<Sex>))]
        public async Task<IActionResult> Get() => Ok(await _sexService.List());

        /// <summary>
        /// Gets a specific Sex entity.
        /// </summary>
        /// <param name="id">The ID of the Sex entity to return.</param>
        /// <returns>A single Sex entity.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(Sex))]
        [SwaggerResponse(404, "Sex with provided id does not exist/")]
        public async Task<IActionResult> Get(int id)
        {
            var sex = await _sexService.Get(id);
            if (sex != null)
                return Ok(sex);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Sex entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Sex to add or update.</param>
        /// <param name="model">Sex object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] Sex model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _sexService.Get(id) == null)
            {
                await _sexService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _sexService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds or updates a Sex entity with an auto-generated identity.
        /// </summary>
        /// <param name="value">Sex name value to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            //check we're maintaining unique values
            if (await _sexService.GetByValue(value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, value);

            var sex = await _sexService.Create(new Sex{Value = value});

            return CreatedAtAction("Get", new { id = sex.Id }, sex);
        }
    }
}
