using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/CollectionStatus")]
    public class CollectionStatusController : ApiBaseController
    {
        private IReferenceDataService<CollectionStatus> _collectionStatusService;

        public CollectionStatusController(IReferenceDataService<CollectionStatus> collectionStatusService)
        {
            _collectionStatusService = collectionStatusService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _collectionStatusService.List())
                    .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadCollectionStatusModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _collectionStatusService.GetUsageCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _collectionStatusService.Get(id);

            // If in use, prevent update
            if (await _collectionStatusService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionStatus", $"The collection status \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionStatusService.Delete(id);

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
            if (await _collectionStatusService.Exists(model.Description))
            {
                ModelState.AddModelError("CollectionStatus", "That collection status already exists!");
            }

            // If in use, prevent update
            if (await _collectionStatusService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionStatus", $"The collection status \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionStatusService.Update(new CollectionStatus
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
            if (await _collectionStatusService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection status descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionStatusService.Add(new CollectionStatus
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
            await _collectionStatusService.Update(new CollectionStatus
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description,
            });

        }

    }
}