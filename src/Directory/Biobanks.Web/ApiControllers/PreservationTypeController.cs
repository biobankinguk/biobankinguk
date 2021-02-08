using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Services.Contracts;
using Biobanks.Web.Models.ADAC;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/PreservationType")]
    public class PreservationTypeController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public PreservationTypeController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListPreservationTypesAsync())
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
            if (await _biobankReadService.ValidPreservationTypeAsync(model.Value, model.StorageTemperatureId))
            {
                ModelState.AddModelError("", "");
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

            await _biobankWriteService.AddPreservationTypeAsync(type);
            await _biobankWriteService.UpdatePreservationTypeAsync(type, true); // Ensure sortOrder is correct

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
            if (await _biobankReadService.ValidPreservationTypeAsync(model.Value, model.StorageTemperatureId))
            {
                ModelState.AddModelError("", "");
            }

            if (await _biobankReadService.IsPreservationTypeInUse(model.Id))
            {
                ModelState.AddModelError("", "");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdatePreservationTypeAsync(new PreservationType()
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
            var model = (await _biobankReadService.ListPreservationTypesAsync()).First(x => x.Id == id);

            // If in use, prevent update
            if (await _biobankReadService.IsPreservationTypeInUse(id))
            {
                ModelState.AddModelError("", "");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeletePreservationTypeAsync(new PreservationType()
            {
                Id = model.Id,
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

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, PreservationTypeModel model)
        {
            await _biobankWriteService.UpdatePreservationTypeAsync(new PreservationType()
            {
                Id = id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                StorageTemperatureId = model.StorageTemperatureId
            },
            true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }
    }
}