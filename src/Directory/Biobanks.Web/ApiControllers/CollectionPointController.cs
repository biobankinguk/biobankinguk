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
    [RoutePrefix("api/CollectionPoint")]
    public class CollectionPointController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CollectionPointController(IBiobankReadService biobankReadService,
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

            // If in use, prevent update
            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("CollectionPoints", $"The Collection points \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
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
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListCollectionPointsAsync()).Where(x => x.CollectionPointId == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionPointInUse(id))
            {
                ModelState.AddModelError("CollectionPoints", $"The collection point \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
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
                success = true,
                name = model.Description,
            });
        }

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, CollectionPointModel model)
        {
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