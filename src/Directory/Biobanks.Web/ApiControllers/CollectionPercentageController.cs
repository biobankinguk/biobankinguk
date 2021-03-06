﻿using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/CollectionPercentage")]
    public class CollectionPercentageController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CollectionPercentageController(IBiobankReadService biobankReadService,
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
            var models = (await _biobankReadService.ListCollectionPercentagesAsync())
                .Select(x =>
                    Task.Run(async () => new CollectionPercentageModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetCollectionPercentageUsageCount(x.Id)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(CollectionPercentageModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionPercentageAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That description is already in use. Collection percentage descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var percentage = new CollectionPercentage
            {
                Id = model.Id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound,
            };

            await _biobankWriteService.AddCollectionPercentageAsync(percentage);
            await _biobankWriteService.UpdateCollectionPercentageAsync(percentage, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, CollectionPercentageModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionPercentageAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That collection percentage already exists!");
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("CollectionPercentage", "That collection percentage is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateCollectionPercentageAsync(new CollectionPercentage
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
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
            var model = (await _biobankReadService.ListCollectionPercentagesAsync()).Where(x => x.Id == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionPercentageInUse(id))
            {
                ModelState.AddModelError("CollectionPercentage", $"The collection percentage \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteCollectionPercentageAsync(new CollectionPercentage
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder,
                LowerBound = 0,
                UpperBound = 1
            });

            // Success
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, CollectionPercentageModel model)
        {
            await _biobankWriteService.UpdateCollectionPercentageAsync(new CollectionPercentage
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            }, true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}