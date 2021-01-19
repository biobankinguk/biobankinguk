﻿using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/SampleCollectionMode")]
    public class SampleCollectionModeController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public SampleCollectionModeController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListSampleCollectionModeAsync())
                .Select(x =>
                    Task.Run(async () => new SampleCollectionModeModel
                    {
                        Id = x.SampleCollectionModeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetSampleCollectionModeUsageCount(x.SampleCollectionModeId)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(SampleCollectionModeModel model)
        {
            //// Validate model
            if (await _biobankReadService.ValidSampleCollectionModeAsync(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That description is already in use. Sample collection modes must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddSampleCollectionModeAsync(mode);
            await _biobankWriteService.UpdateSampleCollectionModeAsync(mode, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, SampleCollectionModeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidSampleCollectionModeAsync(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That sample collection modes already exists!");
            }

            if (await _biobankReadService.IsSampleCollectionModeInUse(id))
            {
                ModelState.AddModelError("SampleCollectionModes", $"This sample collection mode \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            
           var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateSampleCollectionModeAsync(mode);

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
            var model = (await _biobankReadService.ListSampleCollectionModeAsync()).Where(x => x.SampleCollectionModeId == id).First();

            if (await _biobankReadService.IsSampleCollectionModeInUse(id))
            {
                ModelState.AddModelError("SampleCollectionModes", $"This sample collection mode \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = model.SampleCollectionModeId,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.DeleteSampleCollectionModeAsync(mode);

            // Success
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, SampleCollectionModeModel model)
        {
            var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateSampleCollectionModeAsync(mode, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }
    }
}