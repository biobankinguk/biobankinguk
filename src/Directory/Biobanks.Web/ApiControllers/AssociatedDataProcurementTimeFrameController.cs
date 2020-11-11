using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.Shared;
using Biobanks.Web.Utilities;
using Biobanks.Web.Models.ADAC;
using System.Web.Http.Results;
using System.Collections;
using System.Web.Http.ModelBinding;
using Directory.Services.Contracts;
using System.Linq;

namespace Biobanks.Web.ApiControllers
{
    public class AssociatedDataProcurementTimeFrameController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public AssociatedDataProcurementTimeFrameController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET api/{Controller}/{Action}
        // GET: AssociatedDataProcurementTimeFrame
        [HttpGet]
        public async Task<IList> AssociatedDataProcurementTimeFrame()
        {
            var models = (await _biobankReadService.ListAssociatedDataProcurementTimeFrames())
                    .Select(x =>

                Task.Run(async () => new ReadAssociatedDataProcurementTimeFrameModel
                {
                    Id = x.AssociatedDataProcurementTimeframeId,
                    Description = x.Description,
                    DisplayName = x.DisplayValue,
                    CollectionCapabilityCount = await _biobankReadService.GetAssociatedDataProcurementTimeFrameCollectionCapabilityCount(x.AssociatedDataProcurementTimeframeId),
                    SortOrder = x.SortOrder
                }).Result).ToList();

            return models;

        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteAssociatedDataProcurementTimeFrame(Models.Shared.AssociatedDataProcurementTimeFrameModel model)
        {
            //Validate min amount of time frames
            var timeFrames = await _biobankReadService.ListAssociatedDataProcurementTimeFrames();
            if (timeFrames.Count() <= 2)
            {
                return Json(new
                {
                    msg = $"A minimum amount of 2 time frames are allowed.",
                    type = FeedbackMessageType.Warning
                });
            }

            if (await _biobankReadService.IsAssociatedDataProcurementTimeFrameInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The associated data procurement time frame \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteAssociatedDataProcurementTimeFrameAsync(new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The associated data procurement time frame \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });

        }

        [HttpPost]
        public async Task<IHttpActionResult> EditAssociatedDataProcurementTimeFrameAjax(Models.Shared.AssociatedDataProcurementTimeFrameModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidAssociatedDataProcurementTimeFrameDescriptionAsync(model.Id, model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That Associated Data Procurement Time Frame already exists!");
            }

            if (model.DisplayName == null)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field is required.");
            }

            if (model.DisplayName.Length > 10)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field allows a maximum of 10 characters.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsAssociatedDataProcurementTimeFrameInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateAssociatedDataProcurementTimeFrameAsync(new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = model.Id,
                Description = model.Description,
                DisplayValue = model.DisplayName,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditAssociatedDataProcurementTimeFrameSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddAssociatedDataProcurementTimeFrameAjax(Models.Shared.AssociatedDataProcurementTimeFrameModel model)
        {
            // Validate model
            var timeFrames = await _biobankReadService.ListAssociatedDataProcurementTimeFrames();
            if (timeFrames.Count() >= 5)
            {
                //SetTemporaryFeedbackMessage($"A maximum amount of 5 time frames are allowed.", FeedbackMessageType.Warning);
                return Json(new
                {
                    msg = $"A maximum amount of 5 time frames are allowed.",
                    type = "warning",
                    success = true,
                    redirect = $"AssociatedDataProcurementTimeFrame"
                });
            }

            if (await _biobankReadService.ValidAssociatedDataProcurementTimeFrameDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "That description is already in use. Associated Data Procurement Time Frame descriptions must be unique.");
            }

            if (model.DisplayName == null)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field is required.");
            }

            if (model.DisplayName.Length > 10)
            {
                ModelState.AddModelError("AssociatedDataProcurementTimeFrame", "The Display Name field allows a maximum of 10 characters.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var procurement = new AssociatedDataProcurementTimeframe
            {
                AssociatedDataProcurementTimeframeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                DisplayValue = model.DisplayName
            };

            await _biobankWriteService.AddAssociatedDataProcurementTimeFrameAsync(procurement);
            await _biobankWriteService.UpdateAssociatedDataProcurementTimeFrameAsync(procurement, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddAssociatedDataProcurementTimeFrameSuccess?name={model.Description}"
            });
        }

        private IHttpActionResult JsonModelInvalidResponse(ModelStateDictionary state)
        {
            return Json(new
            {
                success = false,
                errors = state.Values
                    .Where(x => x.Errors.Count > 0)
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

    }
}