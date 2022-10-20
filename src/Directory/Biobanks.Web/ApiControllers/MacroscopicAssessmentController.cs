using System;
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
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.ApiControllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    [UserApiAuthorize(Roles = "ADAC")]
    [RoutePrefix("api/MacroscopicAssessment")]
    public class MacroscopicAssessmentController : ApiBaseController
    {
        private readonly IReferenceDataService<MacroscopicAssessment> _macroscopicAssessmentService;
        private readonly IConfigService _configService;

        public MacroscopicAssessmentController(
            IReferenceDataService<MacroscopicAssessment> macroscopicAssessment, 
            IConfigService configService)
        {
            _macroscopicAssessmentService = macroscopicAssessment;
            _configService = configService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("")]
        public async Task<IList> Get()
        {
            var models = (await _macroscopicAssessmentService.List())
                .Select(x =>
                    Task.Run(async () => new MacroscopicAssessmentModel()
                    {
                        Id = x.Id,
                        Description = x.Value,
                        SortOrder = x.SortOrder,
                        SampleSetsCount = await _macroscopicAssessmentService.GetUsageCount(x.Id)
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
            if (await _macroscopicAssessmentService.Exists(model.Description))
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

            await _macroscopicAssessmentService.Add(assessment);
            await _macroscopicAssessmentService.Update(assessment);

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
            if (await _macroscopicAssessmentService.Exists(model.Description))
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

            await _macroscopicAssessmentService.Update(new MacroscopicAssessment
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

            var model = await _macroscopicAssessmentService.Get(id);

            //Getting the name of the reference type as stored in the config
            Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            if (await _macroscopicAssessmentService.IsInUse(id))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"This {currentReferenceName.Value} \"{model.Value}\" is currently in use and cannot be deleted.");
            }

            if ((await _macroscopicAssessmentService.Count()) <= 1)
            {
                ModelState.AddModelError("MacroscopicAssessments", $"The {currentReferenceName.Value} \"{model.Value}\" is currently the last entry and cannot be deleted.");
            }

            if (!ModelState.IsValid)
            {
                return JsonModelInvalidResponse(ModelState);
            }

            await _macroscopicAssessmentService.Delete(id);

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
            await _macroscopicAssessmentService.Update(new MacroscopicAssessment
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