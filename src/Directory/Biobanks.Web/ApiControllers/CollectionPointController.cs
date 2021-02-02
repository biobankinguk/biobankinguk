using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;

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
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetCollectionPointUsageCount(x.Id)
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
                Id = model.Id,
                Value = model.Description,
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

            await _biobankWriteService.UpdateCollectionPointAsync(new CollectionPoint
            {
                Id = id,
                Value = model.Description,
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
            var model = (await _biobankReadService.ListCollectionPointsAsync()).Where(x => x.Id == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionPointInUse(id))
            {
                ModelState.AddModelError("CollectionPoints", $"The collection point \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteCollectionPointAsync(new CollectionPoint
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder
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
        public async Task<IHttpActionResult> Move(int id, CollectionPointModel model)
        {
            await _biobankWriteService.UpdateCollectionPointAsync(new CollectionPoint
            {
                Id = id,
                Value = model.Description,
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