using Directory.Services.Contracts;
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
using Directory.Data.Constants;

namespace Biobanks.Web.ApiControllers
{
    public class HtaStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public HtaStatusController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }


        // GET: HtaStatus
        [HttpGet]
        public async Task<IList> HtaStatus()
        {
            var models = (await _biobankReadService.ListHtaStatusesAsync())
                        .Select(x =>

                    Task.Run(async () => new ReadHtaStatusModel
                    {
                        Id = x.HtaStatusId,
                        Description = x.Description,
                        CollectionCount = await _biobankReadService.GetHtaStatusCollectionCount(x.HtaStatusId),
                        SortOrder = x.SortOrder
                    }).Result)

                        .ToList();

         return models;

        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteHtaStatus(Models.Shared.HtaStatusModel model)
        {
            if (await _biobankReadService.IsHtaStatusInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The hta status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The hta status \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditHtaStatusAjax(Models.Shared.HtaStatusModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidHtaStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("HtaStatus", "That hta status already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsHtaStatusInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateHtaStatusAsync(new HtaStatus
            {
                HtaStatusId = model.Id,
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
        public async Task<IHttpActionResult> AddHtaStatusAjax(Models.Shared.HtaStatusModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidHtaStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Hta status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddHtaStatusAsync(new HtaStatus
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