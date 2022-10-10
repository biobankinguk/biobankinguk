using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Submissions.Api.Services.Directory;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Models.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Entities.Data;
using Biobanks.Submissions.Api.Config;

namespace Biobanks.Submissions.Api.Controllers.ReferenceData
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Reference Data")]
    public class MacroscopicAssessmentController : ControllerBase
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
        [AllowAnonymous]
        public async Task<ActionResult> Post(MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

            // Validate model
            if (await _macroscopicAssessmentService.Exists(model.Description))
            {
                ModelState.AddModelError("MacroscopicAssessments", $"That description is already in use. {currentReferenceName.Value} descriptions must be unique.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
            return Ok(model);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Put(int id, MacroscopicAssessmentModel model)
        {
            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

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
                return BadRequest(ModelState);
            }

            await _macroscopicAssessmentService.Update(new MacroscopicAssessment
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            // Success message
            return Ok(model);
        }

        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> Delete(int id)
        {

            var model = await _macroscopicAssessmentService.Get(id);

            //Getting the name of the reference type as stored in the config
            Entities.Data.Config currentReferenceName = await _configService.GetSiteConfig(ConfigKey.MacroscopicAssessmentName);

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
                return BadRequest(ModelState);
            }

            await _macroscopicAssessmentService.Delete(id);

            // Success
            return Ok(model);
        }

        [HttpPost("{id}/move")]
        [AllowAnonymous]
        public async Task<ActionResult> Move(int id, MacroscopicAssessmentModel model)
        {
            await _macroscopicAssessmentService.Update(new MacroscopicAssessment
            {
                Id = id,
                Value = model.Description,
                SortOrder = model.SortOrder
            });

            //Everything went A-OK!
            return Ok(model);
        }
    }
}

