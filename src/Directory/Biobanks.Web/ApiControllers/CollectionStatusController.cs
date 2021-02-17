using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Web.ApiControllers
{
    [System.Web.Http.Authorize(Roles = "ADAC")]
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
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListCollectionStatusesAsync())
                    .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadCollectionStatusModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _biobankReadService.GetCollectionStatusCollectionCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListCollectionStatusesAsync()).Where(x => x.Id == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionStatusInUse(id))
            {
                ModelState.AddModelError("CollectionStatus", $"The collection status \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteCollectionStatusAsync(new CollectionStatus
            {
                Id = model.Id,
                Value = model.Value
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
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

            await _biobankWriteService.UpdateCollectionStatusAsync(new CollectionStatus
            {
                Id = model.Id,
                Value = model.Description,
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
                Value = model.Description,
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
            await _biobankWriteService.UpdateCollectionStatusAsync(new CollectionStatus
            {
                Id = id,
                Value = model.Description,
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