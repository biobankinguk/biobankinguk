using Directory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Directory.Data.Constants;

namespace Biobanks.Web.ApiControllers
{
    [RoutePrefix("api/MacroscopicAssessment")]
    public class MacroscopicAssessmentController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;

        public MacroscopicAssessmentController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IList> Get()
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
        [Route("")]
        public async Task<IHttpActionResult> Post(MacroscopicAssessmentModel model)
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

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(int id, MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            // Validate model
            if (await _biobankReadService.ValidMacroscopicAssessmentAsync(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (model.SampleSetsCount > 0)
            {
                ModelState.AddModelError("MacroscopicAssessments", $"This {currentReferenceName.Value} \"{model.Description}\" is currently in use and cannot be edited.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.UpdateMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                MacroscopicAssessmentId = id,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success message
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(int id)
        {

            var model = (await _biobankReadService.ListMacroscopicAssessmentsAsync()).Where(x => x.MacroscopicAssessmentId == id).First();
            
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _biobankReadService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            if (await _biobankReadService.IsMacroscopicAssessmentInUse(id))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"This {currentReferenceName.Value} \"{model.Description}\" is currently in use and cannot be deleted.");
            }

            if ((await _biobankReadService.ListMacroscopicAssessmentsAsync()).Count() <= 1)
            {
                ModelState.AddModelError("MacroscopicAssessments", $"The {currentReferenceName.Value} \"{model.Description}\" is currently the last entry and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                MacroscopicAssessmentId = model.MacroscopicAssessmentId,
                Description = model.Description,
                SortOrder = model.SortOrder
            });

            // Success
            return Json(new
            {
                success = true,
                name = model.Description,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, MacroscopicAssessmentModel model)
        {
            await _biobankWriteService.UpdateMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                MacroscopicAssessmentId = id,
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