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
    public class MacroscopicAssessmentsController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public MacroscopicAssessmentsController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        // GET: MacroscopicAssessment
        [HttpGet]
        public async Task<IList> MacroscopicAssessments()
        {
            var models = (await _biobankReadService.ListMacroscopicAssessmentsAsync())
                .Select(x =>
                    Task.Run(async () => new MacroscopicAssessmentModel()
                    {
                        Id = x.MacroscopicAssessmentId,
                        Description = x.Description,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetMacroscopicAssessmentUsageCount(x.MacroscopicAssessmentId)
                    })
                    .Result
                )
                .ToList();

            return models;
        }

        [HttpPost]
        public async Task<IHttpActionResult> AddMacroscopicAssessmentAjax(MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            // Validate model
            if (await _biobankReadService.ValidMacroscopicAssessmentAsync(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            var assessment = new MacroscopicAssessment
            {
                MacroscopicAssessmentId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            };

            await _biobankWriteService.AddMacroscopicAssessmentAsync(assessment);
            await _biobankWriteService.UpdateMacroscopicAssessmentAsync(assessment, true);

            // Success response
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"AddMacroscopicAssessmentSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> EditMacroscopicAssessmentAjax(MacroscopicAssessmentModel model, bool sortOnly = false)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            //// Validate model
            if (!sortOnly && await _biobankReadService.ValidMacroscopicAssessmentAsync(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            // If in use, then only re-order the type
            bool inUse = model.SampleSetsCount > 0;

            // Update Preservation Type
            await _biobankWriteService.UpdateMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                MacroscopicAssessmentId = model.Id,
                Description = model.Description,
                SortOrder = model.SortOrder
            },
            (sortOnly || inUse));

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
                redirect = $"EditMacroscopicAssessmentSuccess?name={model.Description}&referencename={currentReferenceName.Value}"
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> DeleteMacroscopicAssessment(MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            if (await _biobankReadService.IsMacroscopicAssessmentInUse(model.Id))
            {
                return Json(new
                {
                    msg = $"The {currentReferenceName.Value} \"{model.Description}\" is currently in use, and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            if ((await _biobankReadService.ListMacroscopicAssessmentsAsync()).Count() <= 1)
            {
                return Json(new
                {
                    msg = $"The {currentReferenceName.Value} \"{model.Description}\" is currently the last entry and cannot be deleted.",
                    type = FeedbackMessageType.Danger
                });
            }

            await _biobankWriteService.DeleteMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                MacroscopicAssessmentId = model.Id,
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