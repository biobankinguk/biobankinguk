using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Directory.Data.Constants;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/StorageTemperature")]
    public class StorageTemperatureController : ApiBaseController
    {
        private readonly IReferenceDataService<StorageTemperature> _storageTemperatureService;
        private readonly IConfigService _configService;

        public StorageTemperatureController(
            IReferenceDataService<StorageTemperature> storageTemperatureService,
            IConfigService configService)
        {
            _storageTemperatureService = storageTemperatureService;
            _configService = configService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _storageTemperatureService.List())
                .Select(x =>
                    Task.Run(async () =>
                        new StorageTemperatureModel()
                        {
                            Id = x.Id,
                            Value = x.Value,
                            SortOrder = x.SortOrder,
                            InUse = await _storageTemperatureService.IsInUse(x.Id)
                        }
                    ).Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(StorageTemperatureModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.StorageTemperatureName);

            // Validate model
            if (await _storageTemperatureService.Exists(model.Value))
            {
                ModelState.AddModelError("StorageTemperature", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var type = new StorageTemperature
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder
            };

            await _storageTemperatureService.Add(type);
            await _storageTemperatureService.Update(type); // Ensure sortOrder is correct

            // Success response
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, StorageTemperatureModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.StorageTemperatureName);

            // Validate model
            if (await _storageTemperatureService.Exists(model.Value))
            {
                ModelState.AddModelError("StorageTemperature", $"That {currentReferenceName.Value} already exists!");
            }

            // If in use, prevent update
            if (model.InUse)
            {
                ModelState.AddModelError("StorageTemperature", $"The storage temperature \"{model.Value}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _storageTemperatureService.Update(new StorageTemperature
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder
            });

            // Success message
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _storageTemperatureService.Get(id);

            // If in use, prevent update
            if (await _storageTemperatureService.IsInUse(id))
            {
                ModelState.AddModelError("StorageTemperature", $"The storage temperature \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _storageTemperatureService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, StorageTemperatureModel model)
        {
            await _storageTemperatureService.Update(new StorageTemperature
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });

        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{storageTemperature}/preservationtype")]
        public async Task<IList> GetValidPreservationTypes(int storageTemperature)
            => (await _biobankReadService.ListPreservationTypesAsync()).Where(x => x.StorageTemperatureId == storageTemperature).Select(x => x.Id).ToList();
    }
}