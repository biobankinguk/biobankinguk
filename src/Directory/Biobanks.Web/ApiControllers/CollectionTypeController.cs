﻿using Directory.Services.Contracts;
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
    [RoutePrefix("api/CollectionType")]
    public class CollectionTypeController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public CollectionTypeController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListCollectionTypesAsync())
                    .Select(x =>

                Task.Run(async () => new ReadCollectionTypeModel
                {
                    Id = x.CollectionTypeId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetCollectionTypeCollectionCount(x.CollectionTypeId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListCollectionTypesAsync()).Where(x => x.CollectionTypeId == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionTypeInUse(id))
            {
                ModelState.AddModelError("CollectionType", $"The Collection type \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteCollectionTypeAsync(new CollectionType
            {
                CollectionTypeId = model.CollectionTypeId,
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
        public async Task<IHttpActionResult> Put(int id, Models.Shared.CollectionTypeModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidCollectionTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("CollectionType", "That collection type already exists!");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsCollectionTypeInUse(id))
            {
                ModelState.AddModelError("CollectionType", $"The Collection type \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionTypeAsync(new CollectionType
            {
                CollectionTypeId = id,
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
        public async Task<IHttpActionResult> Post(Models.Shared.CollectionTypeModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidCollectionTypeDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Collection types descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddCollectionTypeAsync(new CollectionType
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

        [HttpPut]
        [Route("Sort/{id}")]
        public async Task<IHttpActionResult> Sort(int id, Models.Shared.CollectionTypeModel model)
        {
            // Update Preservation Type
            await _biobankWriteService.UpdateCollectionTypeAsync(new CollectionType
            {
                CollectionTypeId = id,
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