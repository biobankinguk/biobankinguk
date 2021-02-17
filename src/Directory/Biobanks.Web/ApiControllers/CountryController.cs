using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Web.ApiControllers
{
    [System.Web.Http.Authorize(Roles = "ADAC")]
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
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListCountriesAsync())
                .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadCountryModel
                {
                    Id = x.Id,
                    Name = x.Value,
                    CountyOrganisationCount = await _biobankReadService.GetCountryCountyOrganisationCount(x.Id)
                }).Result)

                .ToList();

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCountryNameAsync(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Country names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddCountryAsync(new Country
            {
                Value = model.Name
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
        public async Task<IHttpActionResult> Put(int id, CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCountryNameAsync(id, model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use by another country. Country names must be unique.");
            }

            if (await _biobankReadService.IsCountryInUse(id))
            {
                ModelState.AddModelError("Name", "This country is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateCountryAsync(new Country
            {
                Id = id,
                Value = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListCountriesAsync()).Where(x => x.Id == id).First();

            if (await _biobankReadService.IsCountryInUse(id))
            {
                ModelState.AddModelError("Name", $"The country \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteCountryAsync(new Country
            {
                Id = model.Id,
                Value = model.Value
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }
    }
}