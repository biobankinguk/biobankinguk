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
    [RoutePrefix("api/ConsentRestriction")]
    public class ConsentRestrictionController : ApiBaseController
    {
        private readonly IReferenceDataService<ConsentRestriction> _consentRestrictionService;

        public ConsentRestrictionController(IReferenceDataService<ConsentRestriction> consentRestrictionService)
        {
            _consentRestrictionService = consentRestrictionService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var model = (await _consentRestrictionService.List())
                    .Select(x =>

                Task.Run(async () => new Models.ADAC.ReadConsentRestrictionModel
                {
                    Id = x.Id,
                    Description = x.Value,
                    CollectionCount = await _consentRestrictionService.GetUsageCount(x.Id),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {
            var model = await _consentRestrictionService.Get(id);

            // If in use, prevent update
            if (await _consentRestrictionService.IsInUse(id))
            {
                ModelState.AddModelError("ConsentRestriction", $"The consent restriction \"{model.Value}\" is currently in use, and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _consentRestrictionService.Delete(id);

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Value
            });
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, ConsentRestrictionModel model)
        {
            // Validate model
            if (await _consentRestrictionService.Exists(model.Description))
            {
                ModelState.AddModelError("ConsentRestriction", "That consent restriction already exists!");
            }

            // If in use, prevent update
            if (await _consentRestrictionService.IsInUse(id))
            {
                ModelState.AddModelError("ConsentRestriction", $"The consent restriction \"{model.Description}\" is currently in use, and cannot be updated.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _consentRestrictionService.Update(new ConsentRestriction
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
        public async Task<IHttpActionResult> Post(ConsentRestrictionModel model)
        {
            //If this description is valid, it already exists
            if (await _consentRestrictionService.Exists(model.Description))
            {
                ModelState.AddModelError("Description", "That description is already in use. Consent restriction descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _consentRestrictionService.Add(new ConsentRestriction
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
        public async Task<IHttpActionResult> Move(int id, ConsentRestrictionModel model)
        {
            await _consentRestrictionService.Update(new ConsentRestriction
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