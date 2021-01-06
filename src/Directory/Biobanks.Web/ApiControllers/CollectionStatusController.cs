using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Directory.Entity.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/CollectionStatus")]
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

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListCollectionStatusesAsync())
                    .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadCollectionStatusModel
                {
                    Id = x.CollectionStatusId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetCollectionStatusCollectionCount(x.CollectionStatusId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListCollectionStatusesAsync()).Where(x => x.CollectionStatusId == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionStatusInUse(id))
            {
                ModelState.AddModelError("CollectionStatus", $"The collection status \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteCollectionStatusAsync(new CollectionStatus
            {
                CollectionStatusId = model.CollectionStatusId,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, CollectionStatusModel model, bool sortOnly = false)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionStatusDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionStatus", "That collection status already exists!");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionStatusInUse(id))
            {
                ModelState.AddModelError("CollectionStatus", $"The collection status \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionStatusAsync(new CollectionStatus
            {
                CollectionStatusId = model.Id,
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

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(CollectionStatusModel model)
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

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, CollectionStatusModel model)
        {
            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionStatusAsync(new CollectionStatus
            {
                CollectionStatusId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            }, 
            true);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}