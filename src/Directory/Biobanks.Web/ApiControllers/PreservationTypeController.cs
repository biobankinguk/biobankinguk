﻿using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Directory.Data.Constants;

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
            var models = (await _biobankReadService.ListPreservationTypesAsync())
                .Select(x =>
                    new PreservationTypeModel()
                    {
                        Id = x.PreservationTypeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                    }
                )
                .ToList();

            // Fetch Sample Set Count
            foreach (var model in models)
            {
                model.SampleSetsCount = await _biobankReadService.GetPreservationTypeUsageCount(model.Id);
            }

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(PreservationTypeModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);

            // Validate model
            if (await _biobankReadService.ValidPreservationTypeAsync(model.Description))
            {
                ModelState.AddModelError("PreservationType", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Add new Preservation Type
            var type = new PreservationType
            {
                PreservationTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddPreservationTypeAsync(type);
            await _biobankWriteService.UpdatePreservationTypeAsync(type, true); // Ensure sortOrder is correct

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, PreservationTypeModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);

            // Validate model
            if (await _biobankReadService.ValidPreservationTypeAsync(model.Description))
            {
                ModelState.AddModelError("PreservationType", $"That {currentReferenceName.Value} already exists!");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("PreservationType", $"The preservation type \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Update Preservation Type
            await _biobankWriteService.UpdatePreservationTypeAsync(new PreservationType
            {
                PreservationTypeId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListPreservationTypesAsync()).Where(x => x.PreservationTypeId == id).First();

            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);

            // If in use, prevent update
            if (await _biobankReadService.IsPreservationTypeInUse(id))
            {
                ModelState.AddModelError("PreservationType", $"The preservation type \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeletePreservationTypeAsync(new PreservationType
            {
                PreservationTypeId = model.PreservationTypeId,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, PreservationTypeModel model)
        {
            await _biobankWriteService.UpdatePreservationTypeAsync(new PreservationType
            {
                PreservationTypeId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }
    }
}