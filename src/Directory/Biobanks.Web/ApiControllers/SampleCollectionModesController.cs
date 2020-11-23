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
    public class SampleCollectionModesController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public SampleCollectionModesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: SampleCollectionModes
        [HttpGet]
        public async Task<IList> SampleCollectionModes()
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
        public async Task<IHttpActionResult> AddSampleCollectionModeAjax(SampleCollectionModeModel model)
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
                redirect = $"AddSampleCollectionModeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditSampleCollectionModeAjax(SampleCollectionModeModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidSampleCollectionModeAsync(model.Description))
            {
                ModelState.AddModelError("SampleCollectionModes", "That sample collection modes already exists!");
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

            await _biobankWriteService.UpdateSampleCollectionModeAsync(mode);

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditSampleCollectionModeSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteSampleCollectionMode(SampleCollectionModeModel model)
        {
            if (await _biobankReadService.IsSampleCollectionModeInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The sample collection mode \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            var mode = new SampleCollectionMode
            {
                SampleCollectionModeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.DeleteSampleCollectionModeAsync(mode);

            // Success
            return Json(new
            {
                msg = $"The sample colelction mode  \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success 
            });
        }
    }
}