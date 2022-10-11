using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class CountyController : ControllerBase
    {
        private readonly IReferenceDataService<County> _countyService;
        private readonly IReferenceDataService<Country> _countryService;

        public CountyController(
            IReferenceDataService<County> countyService,
            IReferenceDataService<Country> countryService)
        {
            _countyService = countyService;
            _countryService = countryService;
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpPost]
        [AllowAnonymous]
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

        [HttpPut("{id}")]
        [AllowAnonymous]
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

        [HttpDelete("{id}")]
        [AllowAnonymous]
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

        [HttpGet("{id}/country")]
        [AllowAnonymous]
        public async Task<int> GetCountryId(int id)
        {
            var county = await _countyService.Get(id);

            if (county is null)
                throw new KeyNotFoundException("No County exists with given Id");

            return county.CountryId ?? throw new Exception("This county does not have an associated Country");
        }
    }
}

