using Biobanks.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Biobanks.Entities.Data;
using Biobanks.Web.Models.ADAC;
using System.Collections;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Directory.Data.Constants;
using Biobanks.Web.Filters;

namespace Biobanks.Web.ApiControllers
{
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/MacroscopicAssessment")]
    public class MacroscopicAssessmentController : ApiBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IConfigService _configService;

        public MacroscopicAssessmentController(IBiobankReadService biobankReadService,
                                          IBiobankWriteService biobankWriteService, IConfigService configService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _configService = configService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _biobankReadService.ListMacroscopicAssessmentsAsync())
                .Select(x =>
                    Task.Run(async () => new MacroscopicAssessmentModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _biobankReadService.GetMacroscopicAssessmentUsageCount(x.Id)
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
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

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
                Id = model.Id,
                Value = model.Description,
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
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

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
                Id = id,
                Value = model.Description,
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

            var model = (await _biobankReadService.ListMacroscopicAssessmentsAsync()).Where(x => x.Id == id).First();
            
            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            if (await _biobankReadService.IsMacroscopicAssessmentInUse(id))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"This {currentReferenceName.Value} \"{model.Value}\" is currently in use and cannot be deleted.");
            }

            if ((await _biobankReadService.ListMacroscopicAssessmentsAsync()).Count() <= 1)
            {
                ModelState.AddModelError("MacroscopicAssessments", $"The {currentReferenceName.Value} \"{model.Value}\" is currently the last entry and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _biobankWriteService.DeleteMacroscopicAssessmentAsync(new MacroscopicAssessment
            {
                Id = model.Id,
                Value = model.Value,
                SortOrder = model.SortOrder
            });

            // Success
            return Json(new
            {
                success = true,
                name = model.Value,
            });
        }

        [HttpPost]
        [Route("{id}/move")]
        public async Task<IHttpActionResult> Move(int id, MacroscopicAssessmentModel model)
        {
            await _biobankWriteService.UpdateMacroscopicAssessmentAsync(new MacroscopicAssessment
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