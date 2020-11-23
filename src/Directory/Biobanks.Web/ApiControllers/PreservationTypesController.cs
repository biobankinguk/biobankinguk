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
using Directory.Data.Constants;

namespace Biobanks.Web.ApiControllers
{
    public class PreservationTypesController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public PreservationTypesController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddPreservationTypeAjax(PreservationTypeModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);

            // Validate model
            if (await _biobankReadService.ValidPreservationTypeAsync(model.Description))
            {
                ModelState.AddModelError("PreservationType", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // Add new Preservation Type
            var type = new PreservationType
            {
                PreservationTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddPreservationTypeAsync(type);
            await _biobankWriteService.UpdatePreservationTypeAsync(type, true); // Ensure sortOrder is correct

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddPreservationTypeSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpGet]
        public async Task<IList> PreservationTypes()
        {
            var models = (await _biobankReadService.ListPreservationTypesAsync())
                .Select(x =>
                    new PreservationTypeModel()
                    {
                        Id = x.PreservationTypeId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                    }
                )
                .ToList();

            // Fetch Sample Set Count
            foreach (var model in models)
            {
                model.SampleSetsCount = await _biobankReadService.GetPreservationTypeUsageCount(model.Id);
            }

            return models;
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditPreservationTypeAjax(PreservationTypeModel model, bool sortOnly = false)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);

            // Validate model
            if (!sortOnly && await _biobankReadService.ValidPreservationTypeAsync(model.Description))
            {
                ModelState.AddModelError("PreservationType", $"That {currentReferenceName.Value} already exists!");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdatePreservationTypeAsync(new PreservationType
            {
                PreservationTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));



            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditPreservationTypeSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeletePreservationType(PreservationTypeModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.PreservationTypeName);
            if (await _biobankReadService.IsPreservationTypeInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The {currentReferenceName.Value} \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeletePreservationTypeAsync(new PreservationType
            {
                PreservationTypeId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            return Json(new
            {
                msg = $"The {currentReferenceName.Value}  \"{model.Description}\" was deleted successfully.",
                type = FeedbackMessageType.Success
            });
        }
    }
}