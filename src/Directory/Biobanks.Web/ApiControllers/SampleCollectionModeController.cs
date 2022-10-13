using Biobanks.Services.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding core version"
        , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/SampleCollectionMode")]
    public class SampleCollectionModeController : ApiBaseController
    {
        private readonly IReferenceDataService<SampleCollectionMode> _sampleCollectionModeService;

        public SampleCollectionModeController(IReferenceDataService<SampleCollectionMode> sampleCollectionModeService)
        {
            _sampleCollectionModeService = sampleCollectionModeService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _sampleCollectionModeService.List())
                .Select(x =>
                    Task.Run(async () => new SampleCollectionModeModel
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _sampleCollectionModeService.GetUsageCount(x.Id)
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
            if (await _sampleCollectionModeService.Exists(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That description is already in use. Sample collection modes must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var mode = new SampleCollectionMode
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _sampleCollectionModeService.Add(mode);
            await _sampleCollectionModeService.Update(mode);

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
            if (await _sampleCollectionModeService.Exists(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That sample collection modes already exists!");
            }

            if (await _sampleCollectionModeService.IsInUse(id))
            {
                ModelState.AddModelError("SampleCollectionModes", $"This sample collection mode \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            
           var mode = new SampleCollectionMode
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _sampleCollectionModeService.Update(mode);

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
            var model = await _sampleCollectionModeService.Get(id);

            if (await _sampleCollectionModeService.IsInUse(id))
            {
                ModelState.AddModelError("SampleCollectionModes", $"This sample collection mode \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _sampleCollectionModeService.Delete(id);

            // Success
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, SampleCollectionModeModel model)
        {
            var mode = new SampleCollectionMode
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            };

            await _sampleCollectionModeService.Update(mode);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }
    }
}