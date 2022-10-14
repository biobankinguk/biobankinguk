using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/County")]
    public class CountyController : ApiBaseController
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
        [Route("")]
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
        [Route("")]
        public async Task<IHttpActionResult> Post(CountyModel model)
        {
            // Validate model
            if (await _countyService.Exists(model.Name))
            {
                ModelState.AddModelError("County", "That name is already in use. County names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var county = new County
            {
                Id = model.Id,
                CountryId = model.CountryId,
                Value = model.Name
            };

            await _countyService.Add(county);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, CountyModel model)
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
                return JsonModelInvalidResponse(ModelState);
            }

            var county = new County
            {
                Id = id,
                CountryId = model.CountryId,
                Value = model.Name
            };

            await _countyService.Update(county);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _countryService.Get(id)).Counties.First(x => x.CountryId == id);

            if (await _countyService.IsInUse(id))
            {
                ModelState.AddModelError("County", $"The county \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _countyService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id}/country")]
        public async Task<int> GetCountryId(int id)
        {
            var county = await _countyService.Get(id);

            if (county is null)
                throw new KeyNotFoundException("No County exists with given Id");

            return county.CountryId ?? throw new Exception("This county does not have an associated Country");
        }
    }
}