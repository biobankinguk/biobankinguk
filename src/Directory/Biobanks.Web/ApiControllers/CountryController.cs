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
    public class CountryController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CountryController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        // GET: Country
        [HttpGet]
        public async Task<IList> Country()
        {
            var model = (await _biobankReadService.ListCountriesAsync())
                .Select(x =>

                Task.Run(async () => new ReadCountryModel
                {
                    Id = x.CountryId,
                    Name = x.Name,
                    CountyOrganisationCount = await _biobankReadService.GetCountryCountyOrganisationCount(x.CountryId)
                }).Result)

                .ToList();

            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddCountryAjax(Models.Shared.CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCountryNameAsync(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Country names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .Where(x => x.Errors.Count > 0)
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()
                });
            }

            await _biobankWriteService.AddCountryAsync(new Country
            {
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditCountryAjax(Models.Shared.CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCountryNameAsync(model.Id, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another country. Country names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .Where(x => x.Errors.Count > 0)
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage).ToList()
                });
            }

            if (await _biobankReadService.IsCountryInUse(model.Id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This country is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateCountryAsync(new Country
            {
                CountryId = model.Id,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteCountry(Models.Shared.CountryModel model)
        {
            if (await _biobankReadService.IsCountryInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The country \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteCountryAsync(new Country
            {
                CountryId = model.Id,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The country \"{model.Name}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        private IHttpActionResult JsonModelInvalidResponse(ModelStateDictionary state)
        {
            return Json(new
            {
                success = false,
                errors = state.Values
                    .Where(x => x.Errors.Count > 0)
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}