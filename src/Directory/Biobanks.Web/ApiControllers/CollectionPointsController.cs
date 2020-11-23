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
    public class CollectionPointsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CollectionPointsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: CollectionPoints
        [HttpGet]
        public async Task<IList> CollectionPoints()
        {
            var models = (await _biobankReadService.ListCollectionPointsAsync())
                .Select(x =>
                    Task.Run(async () => new CollectionPointModel()
                    {
                        Id = x.CollectionPointId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                    })
                    .Result
                )
                .ToList();
            return models;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddCollectionPointAjax(CollectionPointModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionPointDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPoints", "That description is already in use. collection point descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var points = new CollectionPoint
            {
                CollectionPointId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddCollectionPointAsync(points);
            await _biobankWriteService.UpdateCollectionPointAsync(points, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddCollectionPointSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditCollectionPointAjax(CollectionPointModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidCollectionPointDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPoints", "That collection point already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionPointAsync(new CollectionPoint
            {
                CollectionPointId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditCollectionPointSuccess?name={model.Description}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteCollectionPoint(CollectionPointModel model)
        {
            if (await _biobankReadService.IsCollectionPointInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The collection point \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteCollectionPointAsync(new CollectionPoint
            {
                CollectionPointId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            return Json(new
            {
                msg = $"The collection point  \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }
    }
}