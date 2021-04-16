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

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/StorageTemperature")]
    public class StorageTemperatureController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public StorageTemperatureController(IBiobankReadService biobankReadService,
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
            var models = (await _biobankReadService.ListStorageTemperaturesAsync())
                .Select(x =>
                    new StorageTemperatureModel()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                    }
                )
                .ToList();

            // Fetch Sample Set Count
            foreach (var model in models)
            {
                model.SampleSetsCount = await _biobankReadService.GetStorageTemperatureUsageCount(model.Id);
                model.UsedByPreservationTypes = await _biobankReadService.IsStorageTemperatureAssigned(model.Id);
            }

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(StorageTemperatureModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.StorageTemperatureName);

            // Validate model
            if (await _biobankReadService.ValidStorageTemperatureAsync(model.Value))
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

            await _biobankWriteService.AddStorageTemperatureAsync(type);
            await _biobankWriteService.UpdateStorageTemperatureAsync(type, true); // Ensure sortOrder is correct

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
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.StorageTemperatureName);

            // Validate model
            if (await _biobankReadService.ValidStorageTemperatureAsync(model.Value))
            {
                ModelState.AddModelError("StorageTemperature", $"That {currentReferenceName.Value} already exists!");
            }

            // If in use, prevent update
            if ((model.SampleSetsCount > 0) || model.UsedByPreservationTypes)
            {
                ModelState.AddModelError("StorageTemperature", $"The storage temperature \"{model.Value}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateStorageTemperatureAsync(new StorageTemperature
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
            var model = (await _biobankReadService.ListStorageTemperaturesAsync()).Where(x => x.Id == id).First();

            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.StorageTemperatureName);

            // If in use, prevent update
            if (await _biobankReadService.IsStorageTemperatureInUse(id) || await _biobankReadService.IsStorageTemperatureAssigned(id))
            {
                ModelState.AddModelError("StorageTemperature", $"The storage temperature \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteStorageTemperatureAsync(new StorageTemperature
            {
                Id = model.Id,
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

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, StorageTemperatureModel model)
        {
            await _biobankWriteService.UpdateStorageTemperatureAsync(new StorageTemperature
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder
            },
            true);

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
        {
            var results = (await _biobankReadService.ListPreservationTypesAsync()).Where(x => x.StorageTemperatureId == storageTemperature).Select(x => x.Id).ToList();

            return results;
        }
    }
}