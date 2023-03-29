using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Services.Directory.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Biobanks.Directory.Controllers.ReferenceData
{
    [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class CountyController : ControllerBase
    {
        private readonly IReferenceDataCrudService<County> _countyService;
        private readonly IReferenceDataCrudService<Country> _countryService;

        public CountyController(
            IReferenceDataCrudService<County> countyService,
            IReferenceDataCrudService<Country> countryService)
        {
            _countyService = countyService;
            _countryService = countryService;
        }

        /// <summary>
        /// Generate a list of countries with their counties.
        /// </summary>
        /// <returns>List of countries and counties.</returns>
        /// <response code="200">Request Successful</response>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(CountiesModel))]
        public async Task<CountiesModel> Get()
        {
            var countries = await _countryService.List();

            var model = new CountiesModel
            {
                Counties = countries.ToDictionary(
                        x => x.Value,
                        x => x.Counties.Select(county =>
                            Task.Run(async () =>
                                new CountyModel
                                {
                                    Id = county.Id,
                                    CountryId = x.Id,
                                    Name = county.Value,
                                    CountyUsageCount = await _countyService.GetUsageCount(county.Id)
                                }
                                )
                            .Result
                        )
                    )
            };

            return model;
        }

        /// <summary>
        /// Insert a new county.
        /// </summary>
        /// <param name="model">The county to be inserted.</param>
        /// <returns>The inserted county.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPost]
        [SwaggerResponse(200, Type = typeof(CountyModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Post(CountyModel model)
        {
            // Validate model
            if (await _countyService.Exists(model.Name))
            {
                ModelState.AddModelError("County", "That name is already in use. County names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var county = new County
            {
                Id = model.Id,
                CountryId = model.CountryId,
                Value = model.Name
            };

            await _countyService.Add(county);

            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Update a county.
        /// </summary>
        /// <param name="id">Id of county to update.</param>
        /// <param name="model">The updated values.</param>
        /// <returns>Tjhe updated county.</returns>
        /// <response code="200">Request Successful</response>
        [HttpPut("{id}")]
        [SwaggerResponse(200, Type = typeof(CountyModel))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Put(int id, CountyModel model)
        {
            // Validate model
            if (await _countyService.Exists(model.Name))
            {
                ModelState.AddModelError("County", "That county already exists!");
            }

            if (await _countyService.IsInUse(id))
            {
                ModelState.AddModelError("County", "This county is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var county = new County
            {
                Id = id,
                CountryId = model.CountryId,
                Value = model.Name
            };

            await _countyService.Update(county);

            // Success response
            return Ok(model);
        }

        /// <summary>
        /// Delete a county.
        /// </summary>
        /// <param name="id">Id of the county to delete.</param>
        /// <returns>The Id of the deleted county.</returns>
        /// <response code="200">Request Successful</response>
        [HttpDelete("{id}")]
        [SwaggerResponse(200, Type = typeof(int))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> Delete(int id)
        {
            var model = await _countyService.Get(id);

            if (await _countyService.IsInUse(id))
            {
                ModelState.AddModelError("County", $"The county \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _countyService.Delete(id);

            //Everything went A-OK!
            return Ok(id);
        }

        /// <summary>
        /// Get the country Id of a County Id
        /// </summary>
        /// <param name="id">Id of the county</param>
        /// <returns>The Country Id of the given County Id</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        /// <response code="200">Request Successful</response>
        [HttpGet("{id}/country")]
        [AllowAnonymous]
        [SwaggerResponse(200, Type = typeof(int))]
        [SwaggerResponse(400, "Invalid request.")]
        public async Task<ActionResult> GetCountryId(int id)
        {
            var county = await _countyService.Get(id);

            if (county is null)
            {
                ModelState.AddModelError("County", "No County exists with given Id.");
            }

            if (county?.CountryId is null)
            {
                ModelState.AddModelError("Country", "This County does not have an associated Country.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(county.CountryId);
        }
    }
}

