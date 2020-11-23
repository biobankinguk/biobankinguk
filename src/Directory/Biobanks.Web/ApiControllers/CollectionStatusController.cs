﻿using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Web.Http.Results;
using System.Collections;
using System.Web.Http.ModelBinding;

namespace Biobanks.Web.ApiControllers
{
    public class CollectionStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CollectionStatusController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: CollectionStatus
        [HttpGet]
        public async Task<IList> CollectionStatus()
        {
            var model = (await _biobankReadService.ListCollectionStatusesAsync())
                    .Select(x =>

                Task.Run(async () => new ReadCollectionStatusModel
                {
                    Id = x.CollectionStatusId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetCollectionStatusCollectionCount(x.CollectionStatusId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteCollectionStatus(Models.Shared.CollectionStatusModel model)
        {
            if (await _biobankReadService.IsCollectionStatusInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The collection status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteCollectionStatusAsync(new CollectionStatus
            {
                CollectionStatusId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The collection status \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });

        }

        [HttpPost]
        public async Task<IHttpActionResult> EditCollectionStatusAjax(Models.Shared.CollectionStatusModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionStatus", "That collection status already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsCollectionStatusInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionStatusAsync(new CollectionStatus
            {
                CollectionStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddCollectionStatusAjax(Models.Shared.CollectionStatusModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCollectionStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddCollectionStatusAsync(new CollectionStatus
            {
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }
    }
}