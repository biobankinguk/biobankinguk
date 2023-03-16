using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Auth;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class CountryController : ControllerBase
    {
        private readonly IReferenceDataCrudService<Country> _countryService;

        public CountryController(IReferenceDataCrudService<Country> countryService)
        {
            _countryService = countryService;
        }

        /// <summary>
        /// Generate a list of Countries.
        /// </summary>
        /// <returns>List of countries.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(ReadCountryModel))]
        public async Task<IList> Get()
        {
            var model = (await _countryService.List())
                .Select(x =>

                Task.Run(async () => new ReadCountryModel
                {
                    Id = x.Id,
                    Name = x.Value,
                    CountyOrganisationCount = await _countryService.GetUsageCount(x.Id)
                }).Result)

                .ToList();

            return model;
        }

        /// <summary>
        /// Insert a new country.
        /// </summary>
        /// <param name="model">Model to insert.</param>
        /// <returns>The inserted country.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(CountryModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _countryService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Country names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _countryService.Add(new Country
            {
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Update an existing country.
        /// </summary>
        /// <param name="id">Id of the country to update.</param>
        /// <param name="model">Model with updated values.</param>
        /// <returns>The updated country.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(CountryModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, CountryModel model)
        {
            var exisiting = await _countryService.Get(model.Name);

            //If this description is valid, it already exists
            if (exisiting != null && exisiting.Id != id)
            {
                ModelState.AddModelError("Name", "That name is already in use by another country. Country names must be unique.");
            }

            if (await _countryService.IsInUse(id))
            {
                ModelState.AddModelError("Name", "This country is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _countryService.Update(new Country
            {
                Id = id,
                Value = model.Name
            });

            //Everything went A-OK!
            return Ok(model);
        }

        /// <summary>
        /// Delete an existing country.
        /// </summary>
        /// <param name="id">Id of the country to delete.</param>
        /// <returns>The deleted country.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(CountryModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _countryService.Get(id);

            if (await _countryService.IsInUse(id))
            {
                ModelState.AddModelError("Name", $"The country \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _countryService.Delete(id);

            //Everything went A-OK!
            return Ok(model);
        }
    }
}

