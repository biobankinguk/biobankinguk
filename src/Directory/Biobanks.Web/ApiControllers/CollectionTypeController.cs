using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Web.Filters;
using Biobanks.Directory.Services.Contracts;
using System;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
        " Any changes made here will need to be made in the corresponding service."
        , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/CollectionType")]
    public class CollectionTypeController : ApiBaseController
    {
        private readonly IReferenceDataService<CollectionType> _collectionTypeService;

        public CollectionTypeController(IReferenceDataService<CollectionType> collectionTypeService)
        {
            _collectionTypeService = collectionTypeService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _collectionTypeService.List())
                    .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadCollectionTypeModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _collectionTypeService.GetUsageCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _collectionTypeService.Get(id);

            // If in use, prevent update
            if (await _collectionTypeService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionType", $"The Collection type \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionTypeService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });

        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, CollectionTypeModel model)
        {
            // Validate model
            if (await _collectionTypeService.Exists(model.Description))
            {
                ModelState.AddModelError("CollectionType", "That collection type already exists!");
            }

            // If in use, prevent update
            if (await _collectionTypeService.IsInUse(id))
            {
                ModelState.AddModelError("CollectionType", $"The Collection type \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionTypeService.Update(new CollectionType
            {
                Id = id,
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
        public async Task<IHttpActionResult> Post(CollectionTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _collectionTypeService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection types descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _collectionTypeService.Add(new CollectionType
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
        public async Task<IHttpActionResult> Move(int id, CollectionTypeModel model)
        {
            await _collectionTypeService.Update(new CollectionType
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