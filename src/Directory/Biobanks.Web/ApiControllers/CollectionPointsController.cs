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
    [RoutePrefix("api/CollectionPoints")]
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

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
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
        [Route("")]
        public async Task<IHttpActionResult> Post(CollectionPointModel model)
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
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, CollectionPointModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionPointDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionPoints", "That collection point already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                return Json(new
                {
                    msg = $"The Collection points \"{model.Description}\" is currently in use, and cannot be updated.",
                    type = FeedbackMessageType.Danger
                });
            }

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionPointAsync(new CollectionPoint
            {
                CollectionPointId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> DeleteCollectionPoint(int id)
        {
            var model = (await _biobankReadService.ListCollectionPointsAsync()).Where(x => x.CollectionPointId == id).First();

            if (await _biobankReadService.IsCollectionPointInUse(id))
            {
                return Json(new
                {
                    msg = $"The collection point \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteCollectionPointAsync(new CollectionPoint
            {
                CollectionPointId = model.CollectionPointId,
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

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, CollectionPointModel model)
        {

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateCollectionPointAsync(new CollectionPoint
            {
                CollectionPointId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
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