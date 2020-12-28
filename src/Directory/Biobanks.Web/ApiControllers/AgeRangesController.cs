﻿using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using System.Web.Http.ModelBinding;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/AgeRanges")]
    public class AgeRangesController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AgeRangesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListAgeRangesAsync())
                .Select(x =>
                    Task.Run(async () => new AgeRangeModel()
                    {
                        Id = x.AgeRangeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetAgeRangeUsageCount(x.AgeRangeId)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(AgeRangeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAgeRangeAsync(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Add new Age Range
            var range = new AgeRange
            {
                AgeRangeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddAgeRangeAsync(range);
            await _biobankWriteService.UpdateAgeRangeAsync(range, true); // Ensure sortOrder is correct

            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, AgeRangeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidAgeRangeAsync(model.Description))
            {
                ModelState.AddModelError("AgeRange", "That description is already in use. Age ranges must be unique.");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("AgeRange", $"The age range \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Update Preservation Type
            await _biobankWriteService.UpdateAgeRangeAsync(new AgeRange
            {
                AgeRangeId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListAgeRangesAsync()).Where(x => x.AgeRangeId == id).First();

            if (await _biobankReadService.IsAgeRangeInUse(id))
            {
                return Json(new
                {
                    msg = $"The age range \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAgeRangeAsync(new AgeRange
            {
                AgeRangeId = model.AgeRangeId,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            return Json(new
            {
                msg = $"The age range  \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, AgeRangeModel model)
        {

            var access = new AgeRange
            {
                AgeRangeId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.UpdateAgeRangeAsync(access, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }
    }
}