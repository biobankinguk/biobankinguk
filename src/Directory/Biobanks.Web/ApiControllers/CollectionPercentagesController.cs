using Directory.Services.Contracts;
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
    public class CollectionPercentagesController : ApiController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CollectionPercentagesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        public async Task<IList> CollectionPercentages()
        {
            var models = (await _biobankReadService.ListCollectionPercentagesAsync())
                .Select(x =>
                    Task.Run(async () => new CollectionPercentageModel()
                    {
                        Id = x.CollectionPercentageId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        LowerBound = x.LowerBound,
                        UpperBound = x.UpperBound,
                        SampleSetsCount = await _biobankReadService.GetCollectionPercentageUsageCount(x.CollectionPercentageId)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddCollectionPercentageAjax(CollectionPercentageModel model)
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
                CollectionPercentageId = model.Id,
                Description = model.Description,
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
                redirect = $"AddCollectionPercentageSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditCollectionPercentageAjax(CollectionPercentageModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionPercentageAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPercentage", "That collection percentage already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionPercentageAsync(new CollectionPercentage
            {
                CollectionPercentageId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = model.LowerBound,
                UpperBound = model.UpperBound
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditCollectionPercentageSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteCollectionPercentage(CollectionPercentageModel model)
        {
            if (await _biobankReadService.IsCollectionPercentageInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The collection percentage \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteCollectionPercentageAsync(new CollectionPercentage
            {
                CollectionPercentageId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder,
                LowerBound = 0,
                UpperBound = 1
            });

            // Success
            return Json(new
            {
                msg = $"The collection percentage  \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        // GET: CollectionPercentages
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