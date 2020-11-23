using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Web.Http.Results;
using System.Collections;
using System.Web.Http.ModelBinding;

namespace Biobanks.Web.ApiControllers
{
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


        // GET: County
        [HttpGet]
        public async Task<CountiesModel> County()
        {
                var countries = await _biobankReadService.ListCountriesAsync();
            var model = new CountiesModel
            {
                Counties = countries.ToDictionary(
                        x => x.Name,
                        x => x.Counties.Select(county =>
                            Task.Run(async () =>
                                new CountyModel
                                {
                                    Id = county.CountyId,
                                    CountryId = x.CountryId,
                                    Name = county.Name,
                                    CountyUsageCount = await _biobankReadService.GetCountyUsageCount(county.CountyId)
                                }
                                )
                            .Result
                        )
                    )
            };
            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddCountyAjax(CountyModel model)
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
                CountyId = model.Id,
                CountryId = model.CountryId,
                Name = model.Name
            };

            await _biobankWriteService.AddCountyAsync(county);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"AddCountySuccess?name={model.Name}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditCountyAjax(CountyModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCountyAsync(model.Name))
            {
                ModelState.AddModelError("County", "That county already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            if (await _biobankReadService.IsCountyInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This county is currently in use and cannot be edited." }
                });
            }

            var county = new County
            {
                CountyId = model.Id,
                CountryId = model.CountryId,
                Name = model.Name
            };

            await _biobankWriteService.UpdateCountyAsync(county);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Name,
                redirect = $"EditCountySuccess?name={model.Name}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteCounty(CountyModel model)
        {
            if (await _biobankReadService.IsCountyInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The county \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            var county = new County
            {
                CountyId = model.Id,
                CountryId = model.CountryId,
                Name = model.Name
            };

            await _biobankWriteService.DeleteCountyAsync(county);

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The county type \"{model.Name}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }
    }
}