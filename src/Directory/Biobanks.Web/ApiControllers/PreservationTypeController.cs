using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Contracts;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding core version"
        , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/PreservationType")]
    public class PreservationTypeController : ApiBaseController
    {
        private readonly IReferenceDataService<PreservationType> _preservationTypeService;

        public PreservationTypeController(IReferenceDataService<PreservationType> preservationTypeService)
        {
            _preservationTypeService = preservationTypeService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _preservationTypeService.List())
                .Select(x => 
                    new PreservationTypeModel()
                    {
                        Id = x.Id,
                        Value = x.Value,
                        SortOrder = x.SortOrder,
                        StorageTemperatureId = x.StorageTemperatureId,
                        StorageTemperatureName = x.StorageTemperature?.Value ?? ""
                    }
                )
                .ToList();

            return model;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(PreservationTypeModel model)
        {
            var existing = await _preservationTypeService.Get(model.Value);

            if (existing != null && existing.Id != model.Id)
            {
                ModelState.AddModelError("PreservationTypes", $"That PresevationType is already in use. '{ model.Value }' is already in use at the StorageTemperature.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var type = new PreservationType
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                StorageTemperatureId = model.StorageTemperatureId
            };

            await _preservationTypeService.Add(type);
            await _preservationTypeService.Update(type); // Ensure sortOrder is correct

            // Success response
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, PreservationTypeModel model)
        {
            var existing = await _preservationTypeService.Get(model.Value);

            if (existing != null && existing.Id != model.Id)
            {
                ModelState.AddModelError("PreservationTypes", $"That PreservationType is already in use. '{ model.Value }' is already in use at the StorageTemperature.");
            }

            if (await _preservationTypeService.IsInUse(model.Id))
            {
                ModelState.AddModelError("PreservationTypes", $"Unable to change '{ model.Value }', as it is currently is use.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _preservationTypeService.Update(new PreservationType()
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                StorageTemperatureId = model.StorageTemperatureId
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
            var model = await _preservationTypeService.Get(id);

            // If in use, prevent update
            if (await _preservationTypeService.IsInUse(id))
            {
                ModelState.AddModelError("PreservationTypes", $"Unable to delete '{ model.Value }', as it is currently is use.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _preservationTypeService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, PreservationTypeModel model)
        {
            await _preservationTypeService.Update(new PreservationType()
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                StorageTemperatureId = model.StorageTemperatureId
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }
    }
}