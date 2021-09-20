using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/Country")]
    public class CountryController : ApiBaseController
    {
        private readonly IReferenceDataService<Country> _countryService;

        public CountryController(IReferenceDataService<Country> countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _countryService.List())
                .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadCountryModel
                {
                    Id = x.Id,
                    Name = x.Value,
                    CountyOrganisationCount = await _countryService.GetUsageCount(x.Id)
                }).Result)

                .ToList();

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(CountryModel model)
        {
            //If this description is valid, it already exists
            if (await _countryService.Exists(model.Name))
            {
                ModelState.AddModelError("Name", "That name is already in use. Country names must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _countryService.Add(new Country
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
            var exisiting = _countryService.Get(model.Name);

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
                return JsonModelInvalidResponse(ModelState);
            }

            await _countryService.Update(new Country
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
            var model = await _countryService.Get(id);

            if (await _countryService.IsInUse(id))
            {
                ModelState.AddModelError("Name", $"The country \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _countryService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }
    }
}