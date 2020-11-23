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
    public class ConsentRestrictionsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public ConsentRestrictionsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }
        // GET: ConsentRestrictions
        [HttpGet]
        public async Task<IList> ConsentRestriction()
        {
            var model = (await _biobankReadService.ListConsentRestrictionsAsync())
                    .Select(x =>

                Task.Run(async () => new ReadConsentRestrictionModel
                {
                    Id = x.ConsentRestrictionId,
                    Description = x.Description,
                    CollectionCount = await _biobankReadService.GetConsentRestrictionCollectionCount(x.ConsentRestrictionId),
                    SortOrder = x.SortOrder
                }).Result)

                    .ToList();

            return model;
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteConsentRestriction(Models.Shared.ConsentRestrictionModel model)
        {
            if (await _biobankReadService.IsConsentRestrictionInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The consent restriction \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteConsentRestrictionAsync(new ConsentRestriction
            {
                ConsentRestrictionId = model.Id,
                Description = model.Description
            });

            //Everything went A-OK!
            return Json(new
            {
                msg = $"The consent restriction \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditConsentRestrictionAjax(Models.Shared.ConsentRestrictionModel model, bool sortOnly = false)
        {
            // Validate model
            if (!sortOnly && await _biobankReadService.ValidConsentRestrictionDescriptionAsync(model.Description))
            {
                ModelState.AddModelError("ConsentRestriction", "That consent restriction already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }
            // If in use, then only re-order the type
            bool inUse = await _biobankReadService.IsConsentRestrictionInUse(model.Id);

            // Update Preservation Type
            await _biobankWriteService.UpdateConsentRestrictionAsync(new ConsentRestriction
            {
                ConsentRestrictionId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            //Everything went A-OK!
            return Json(new
            {
                success = true,
                name = model.Description
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddConsentRestrictionAjax(Models.Shared.ConsentRestrictionModel model)
        {
            //If this description is valid, it already exists
            if (await _biobankReadService.ValidDiagnosisDescriptionAsync(model.Description))
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
    }
}