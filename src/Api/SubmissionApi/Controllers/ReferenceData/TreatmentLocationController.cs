using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Biobanks.Common.Auth;
using Biobanks.Common.Data.Entities.ReferenceData;
using Biobanks.SubmissionApi.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.SubmissionApi.Controllers.ReferenceData
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class TreatmentLocationController : ControllerBase
    {
        private readonly ITreatmentLocationService _treatmentLocationService;

        /// <inheritdoc />
        public TreatmentLocationController(ITreatmentLocationService treatmentLocationService)
        {
            _treatmentLocationService = treatmentLocationService;
        }

        /// <summary>
        /// Lists all the available Treatment Locations.
        /// </summary>
        /// <returns>A list of Treatment Locations.</returns>
        [HttpGet]
        [SwaggerResponse(200, Type = typeof(IEnumerable<TreatmentLocation>))]
        public async Task<IActionResult> Get() => Ok(await _treatmentLocationService.List());

        /// <summary>
        /// Gets a specific Treatment Location entity.
        /// </summary>
        /// <param name="id">The ID of the Treatment Location entity to return.</param>
        /// <returns>A single Treatment Location entity.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(200, Type = typeof(TreatmentLocation))]
        [SwaggerResponse(404, "Treatment Location with provided id does not exist.")]
        public async Task<IActionResult> Get(int id)
        {
            var treatmentLocation = await _treatmentLocationService.Get(id);
            if (treatmentLocation != null)
                return Ok(treatmentLocation);

            return NotFound();
        }

        /// <summary>
        /// Adds or updates a Treatment Location entity with a given identity.
        /// </summary>
        /// <param name="id">ID of the Treatment Location to add or update.</param>
        /// <param name="model">Treatment Location object to add or update.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Put(int id, [FromBody] TreatmentLocation model)
        {
            model.Id = id;
            
            // insert if it doesn't already exist
            if (await _treatmentLocationService.Get(id) == null)
            {
                await _treatmentLocationService.Create(model);
                return CreatedAtAction("Get", new {id = model.Id}, model);
            }

            // else update existing entity
            await _treatmentLocationService.Update(model);
            return Ok(model);
        }

        /// <summary>
        /// Adds or updates a Treatment Location entity with an auto-generated identity.
        /// </summary>
        /// <param name="value">Treatment Location value to add or update.</param>
        /// <returns></returns>
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]
        [Authorize(Roles = CustomRoles.SuperAdmin)]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            //check we're maintaining unique values
            if (await _treatmentLocationService.GetByValue(value) != null)
                return StatusCode((int)HttpStatusCode.Conflict, value);

            var treatmentLocation = await _treatmentLocationService.Create(new TreatmentLocation{Value = value});

            return CreatedAtAction("Get", new { id = treatmentLocation.Id }, treatmentLocation);
        }
    }
}
