﻿using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Entities.Data;
using Biobanks.Web.Models.Shared;
using System.Collections;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/ConsentRestriction")]
    public class ConsentRestrictionController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public ConsentRestrictionController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _biobankReadService.ListConsentRestrictionsAsync())
                    .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadConsentRestrictionModel
                {
                    Id = x.ConsentRestrictionId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetConsentRestrictionCollectionCount(x.ConsentRestrictionId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = (await _biobankReadService.ListConsentRestrictionsAsync()).Where(x => x.ConsentRestrictionId == id).First();

            // If in use, prevent update
            if (await _biobankReadService.IsConsentRestrictionInUse(id))
            {
                ModelState.AddModelError("ConsentRestriction", $"The consent restriction \"{model.Description}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteConsentRestrictionAsync(new ConsentRestriction
            {
                ConsentRestrictionId = id,
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
        public async Task<IHttpActionResult> Put(int id, ConsentRestrictionModel model)
        {
            // Validate model
            if (await _biobankReadService.ValidConsentRestrictionDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("ConsentRestriction", "That consent restriction already exists!");
            }

            // If in use, prevent update
            if (await _biobankReadService.IsConsentRestrictionInUse(id))
            {
                ModelState.AddModelError("ConsentRestriction", $"The consent restriction \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateConsentRestrictionAsync(new ConsentRestriction
            {
                ConsentRestrictionId = id,
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
        public async Task<IHttpActionResult> Post(ConsentRestrictionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidSnomedTermDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Consent restriction descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.AddConsentRestrictionAsync(new ConsentRestriction
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
        public async Task<IHttpActionResult> Move(int id, ConsentRestrictionModel model)
        {
            await _biobankWriteService.UpdateConsentRestrictionAsync(new ConsentRestriction
            {
                ConsentRestrictionId = id,
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