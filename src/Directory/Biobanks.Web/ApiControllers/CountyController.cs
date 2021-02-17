using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Models.ADAC;

namespace Biobanks.Web.ApiControllers
{
    [System.Web.Http.Authorize(Roles = "ADAC")]
    [RoutePrefix("api/County")]
    public class CountyController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CountyController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<CountiesModel> Get()
        {
                var countries = await _biobankReadService.ListCountriesAsync();
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
                                    CountyUsageCount = await _biobankReadService.GetCountyUsageCount(county.Id)
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
            if (await _biobankReadService.ValidCountyAsync(model.Name))
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

            await _biobankWriteService.AddCountyAsync(county);

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
            if (await _biobankReadService.ValidCountyAsync(model.Name))
            {
                ModelState.AddModelError("County", "That county already exists!");
            }

            if (await _biobankReadService.IsCountyInUse(id))
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

            await _biobankWriteService.UpdateCountyAsync(county);

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
            var model = (await _biobankReadService.ListCountriesAsync()).Select(x => x.Counties.Where(y=>y.Id == id).First()).First();

            if (await _biobankReadService.IsCountyInUse(id))
            {
                ModelState.AddModelError("County", $"The county \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var county = new County
            {
                Id = model.Id,
                CountryId = model.CountryId,
                Value = model.Value
            };

            await _biobankWriteService.DeleteCountyAsync(county);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }
    }
}