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

    [RoutePrefix("api/Country")]
    public class CountryController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CountryController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
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
        [Route("")]
        public async Task<IHttpActionResult> Post(Models.Shared.CountryModel model)
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

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, Models.Shared.CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCountryNameAsync(id, model.Name))
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

            if (await _biobankReadService.IsCountryInUse(id))
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "This country is currently in use and cannot be edited." }
                });
            }

            await _biobankWriteService.UpdateCountryAsync(new Country
            {
                CountryId = id,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListCountriesAsync()).Where(x => x.CountryId == id).First();

            if (await _biobankReadService.IsCountryInUse(id))
            {
                return Json(new
                {
                    msg = $"The country \"{model.Name}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteCountryAsync(new Country
            {
                CountryId = model.CountryId,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The country \"{model.Name}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }
    }
}