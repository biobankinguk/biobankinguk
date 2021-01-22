using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;

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

                Task.Run(async () => new Models.ADAC.ReadCountryModel
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
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListCountriesAsync()).Where(x => x.CountryId == id).First();

            if (await _biobankReadService.IsCountryInUse(id))
            {
                ModelState.AddModelError("Name", $"The country \"{model.Name}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteCountryAsync(new Country
            {
                CountryId = model.CountryId,
                Name = model.Name
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Name
            });
        }
    }
}