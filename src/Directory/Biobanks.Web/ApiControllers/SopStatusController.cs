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

namespace Biobanks.Web.ApiControllers
{
    public class SopStatusController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public SopStatusController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: SopStatus
        [HttpGet]
        public async Task<IList> SopStatus()
        {
            var models = (await _biobankReadService.ListSopStatusesAsync())
                .Select(x =>
                    Task.Run(async () => new SopStatusModel()
                    {
                        Id = x.SopStatusId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                    })
                    .Result
                )
                .ToList();
            return models;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddSopStatusAjax(SopStatusModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidSopStatusAsync(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That description is already in use. Sop status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var status = new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddSopStatusAsync(status);
            await _biobankWriteService.UpdateSopStatusAsync(status, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddSopStatusSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditSopStatusAjax(SopStatusModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidSopStatusAsync(model.Description))
            {
                ModelState.AddModelError("SopStatus", "That sop status already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateSopStatusAsync(new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditSopStatusSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteSopStatus(SopStatusModel model)
        {
            if (await _biobankReadService.IsSopStatusInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The sop status \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteSopStatusAsync(new SopStatus
            {
                SopStatusId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            return Json(new
            {
                msg = $"The sop status  \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }
    }
}